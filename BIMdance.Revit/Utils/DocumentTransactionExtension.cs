namespace BIMdance.Revit.Utils;

public static class DocumentTransactionExtension
{
    public static string ApplicationName { get; set; }
    
    public static async Task AsyncTransactionGroup(
        this Document document, Func<TransactionGroup, Task> task,
        string transactionGroupName = null) =>
        await RevitTask.RunAsync(async () => await document.TransactionGroupTask(task, transactionGroupName));
    
    public static async Task<T> AsyncTransactionGroup<T>(
        this Document document, Func<TransactionGroup, Task<T>> task,
        string transactionGroupName = null) =>
        await RevitTask.RunAsync(async () => await document.TransactionGroupTask(task, transactionGroupName));
    
    public static async Task AsyncTransaction(
        this Document document, Func<Transaction, Task> task,
        string transactionName = null) =>
        await RevitTask.RunAsync(async () => await document.TransactionTask(task, transactionName));
    
    public static async Task<T> AsyncTransaction<T>(
        this Document document, Func<Transaction, Task<T>> task,
        string transactionName = null) =>
        await RevitTask.RunAsync(async () => await document.TransactionTask(task, transactionName));

    private static Task TransactionGroupTask(
        this Document document, Func<TransactionGroup, Task> task,
        string transactionGroupName = null)
    {
        if (TransactionIsOpened(document))
            return task(null);

        using var transactionGroup = NewTransactionGroup(document, transactionGroupName);

        try
        {
            transactionGroup.Start();
            var result = task(transactionGroup);
            Assimilate(transactionGroup);
            return result;
        }
        catch (Exception exception)
        {
            TransactionException(transactionGroup, exception);
            return Task.CompletedTask;
        }
    }

    private static Task<T> TransactionGroupTask<T>(
        this Document document, Func<TransactionGroup, Task<T>> task,
        string transactionGroupName = null)
    {
        if (TransactionIsOpened(document))
            return task(null);

        using var transactionGroup = NewTransactionGroup(document, transactionGroupName);

        try
        {
            transactionGroup.Start();
            var result = task(transactionGroup);
            Assimilate(transactionGroup);
            return result;
        }
        catch (Exception exception)
        {
            TransactionException(transactionGroup, exception);
            return default;
        }
    }
    
    public static void TransactionGroup(
        this Document document, Action<TransactionGroup> action,
        string transactionGroupName = null)
    {
        if (TransactionIsOpened(document))
        {
            action(null);
            return;
        }

        using var transactionGroup = NewTransactionGroup(document, transactionGroupName);

        try
        {
            transactionGroup.Start();
            action(transactionGroup);
            Assimilate(transactionGroup);
        }
        catch (Exception exception)
        {
            TransactionException(transactionGroup, exception);
        }
    }
    
    public static T TransactionGroup<T>(
        this Document document, Func<TransactionGroup, T> func,
        string transactionGroupName = null)
    {
        if (TransactionIsOpened(document))
            return func(null);

        using var transactionGroup = NewTransactionGroup(document, transactionGroupName);

        try
        {
            transactionGroup.Start();
            var result = func(transactionGroup);
            Assimilate(transactionGroup);
            return result;
        }
        catch (Exception exception)
        {
            TransactionException(transactionGroup, exception);
            return default;
        }
    }

    private static Task TransactionTask(
        this Document document, Func<Transaction, Task> task,
        string transactionName = null)
    {
        if (TransactionIsOpened(document))
            return task(null);

        using var transaction = NewTransaction(document, transactionName);

        try
        {
            transaction.Start();
            var result = task(transaction);
            Commit(transaction);
            return result;
        }
        catch (Exception exception)
        {
            TransactionException(transaction, exception);
            return Task.CompletedTask;
        }
    }

    private static Task<T> TransactionTask<T>(
        this Document document, Func<Transaction, Task<T>> task,
        string transactionName = null)
    {
        if (TransactionIsOpened(document))
            return task(null);

        using var transaction = NewTransaction(document, transactionName);

        try
        {
            transaction.Start();
            var result = task(transaction);
            Commit(transaction);
            return result;
        }
        catch (Exception exception)
        {
            TransactionException(transaction, exception);
            return default;
        }
    }
    
    public static void Transaction(
        this Document document, Action<Transaction> action,
        string transactionName = null, bool ignoreFailures = false)
    {
        if (TransactionIsOpened(document))
        {
            action(null);
            return;
        }

        using var transaction = NewTransaction(document, transactionName);

        try
        {
            if (ignoreFailures)
                DeleteAllWarnings(transaction);

            transaction.Start();
            action(transaction);
            Commit(transaction);
        }
        catch (Exception exception)
        {
            TransactionException(transaction, exception);
        }
    }

    public static T Transaction<T>(
        this Document document, Func<Transaction, T> func,
        string transactionName = null, bool ignoreFailures = false)
    {
        if (TransactionIsOpened(document))
            return func(null);

        using var transaction = NewTransaction(document, transactionName);

        try
        {
            if (ignoreFailures)
                DeleteAllWarnings(transaction);

            transaction.Start();
            var result = func(transaction);
            Commit(transaction);
            return result;
        }
        catch (Exception exception)
        {
            TransactionException(transaction, exception);
            return default;
        }
    }

    public static void SubTransaction(
        this Document document, Action<SubTransaction> action)
    {
        using var subTransaction = new SubTransaction(document);

        try
        {
            subTransaction.Start();
            action(subTransaction);
            Commit(subTransaction);
        }
        catch (Exception exception)
        {
            TransactionException(subTransaction, exception);
        }
    }

    public static T SubTransaction<T>(
        this Document document, Func<SubTransaction, T> func)
        where T : class
    {
        using var subTransaction = new SubTransaction(document);

        try
        {
            subTransaction.Start();
            var result = func(subTransaction);
            subTransaction.Commit();
            return result;
        }
        catch (Exception exception)
        {
            TransactionException(subTransaction, exception);
            return null;
        }
    }

    private static bool TransactionIsOpened(this Document document) =>
        document is not null && (document.IsReadOnly || document.IsModifiable);
    
    private static TransactionGroup NewTransactionGroup(Document document, string transactionGroupName) =>
        new(document, $"{ApplicationName}: {transactionGroupName ?? TransactionNames.Change}");
    
    private static Transaction NewTransaction(Document document, string transactionName) =>
        new(document, $"{ApplicationName}: {transactionName ?? TransactionNames.Change}");

    private static void Assimilate(TransactionGroup transactionGroup)
    {
        switch (transactionGroup.GetStatus())
        {
            case TransactionStatus.Started: transactionGroup.Assimilate(); break;
            case TransactionStatus.RolledBack: break;
            default: transactionGroup.RollBack(); break;
        }
    }

    private static void Commit(Transaction transaction)
    {
        switch (transaction.GetStatus())
        {
            case TransactionStatus.Started: transaction.Commit(); break;
            case TransactionStatus.RolledBack: break;
            default: transaction.RollBack(); break;
        }
    }

    private static void Commit(SubTransaction subTransaction)
    {
        switch (subTransaction.GetStatus())
        {
            case TransactionStatus.Started: subTransaction.Commit(); break;
            case TransactionStatus.RolledBack: break;
            default: subTransaction.RollBack(); break;
        }
    }

    private static void DeleteAllWarnings(Transaction transaction)
    {
        var failureHandlingOptions = transaction.GetFailureHandlingOptions();
        failureHandlingOptions.SetFailuresPreprocessor(new DeleteAllWarningsFailuresPreprocessor());
        transaction.SetFailureHandlingOptions(failureHandlingOptions);
    }

    private class DeleteAllWarningsFailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            failuresAccessor.DeleteAllWarnings();
            return FailureProcessingResult.Continue;
        }
    }

    private static void TransactionException(TransactionGroup transactionGroup, Exception exception)
    {
        if (ToRollBack(transactionGroup.GetStatus()))
            transactionGroup.RollBack();
#if DEBUG
        throw exception;
#endif
        var transactionGroupName = transactionGroup.GetName();
        Logger.Error($"{nameof(transactionGroupName)}: {transactionGroupName}");
        Logger.Error(exception);
    }
    
    private static void TransactionException(Transaction transaction, Exception exception)
    {
        if (ToRollBack(transaction.GetStatus()))
            transaction.RollBack();
#if DEBUG
        throw exception;
#endif
        var transactionName = transaction.GetName();
        Logger.Error($"{nameof(transactionName)}: {transactionName}");
        Logger.Error(exception);
    }
    
    private static void TransactionException(SubTransaction subTransaction, Exception exception)
    {
        if (ToRollBack(subTransaction.GetStatus()))
            subTransaction.RollBack();
#if DEBUG
        throw exception;
#endif
        Logger.Error(exception);
    }

    private static bool ToRollBack(TransactionStatus transactionStatus) => transactionStatus
        is TransactionStatus.Started
        or TransactionStatus.Pending
        or TransactionStatus.Error
        or TransactionStatus.Proceed;
}