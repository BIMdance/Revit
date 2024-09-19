// using System;
//
// namespace CoLa.BimEd.Logic.SDK.DataAccess.Context;
//
// public class UnitOfWork : Disposable, IUnitOfWork
// {
//     private readonly Context _context;
//
//     public UnitOfWork(Context context)
//     {
//         _context = context ?? throw new ArgumentNullException(nameof(context));
//     }
//
//     public Context GetContext()
//     {
//         if (Disposed)
//             ThrowObjectDisposedException();
//
//         return _context;
//     }
//
//     public void SaveChanges()
//     {
//         if (Disposed)
//             ThrowObjectDisposedException();
//
//         _context.SaveChanges(null);
//     }
//
//     public void SaveChanges(string transactionName)
//     {
//         if (Disposed)
//             ThrowObjectDisposedException();
//
//         _context.SaveChanges(transactionName);
//     }
//
//     private void ThrowObjectDisposedException()
//     {
//         throw new ObjectDisposedException(
//             this.GetType().Name,
//             "Unit of Work has been disposed.");
//     }
// }