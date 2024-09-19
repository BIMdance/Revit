// using System;
// using System.Collections.Generic;
// using System.Linq;
// using CoLa.BimEd.Logic.SDK.Collections;
//
// namespace CoLa.BimEd.Logic.SDK.DataAccess.Context;
//
// public abstract class Context
// {
//     private bool _isChangesSaving;
//
//     public event ChangesSavingEventHandler ChangesSaving;
//
//     private Dictionary<Type, ISet> _genericSets;
//     public readonly Dictionary<object, EntityEntry> AddedEntityStore;
//     public readonly Dictionary<object, EntityEntry> ModifiedEntityStore;
//     public readonly Dictionary<object, EntityEntry> DeletedEntityStore;
//
//     protected Context()
//     {
//         InitializeSets();
//
//         AddedEntityStore = new Dictionary<object, EntityEntry>();
//         ModifiedEntityStore = new Dictionary<object, EntityEntry>();
//         DeletedEntityStore = new Dictionary<object, EntityEntry>();
//     }
//
//     public bool HasAnyChanges() => AddedEntityStore.Any() || DeletedEntityStore.Any() || ModifiedEntityStore.Any();
//
//     public void InitializeSets()
//     {
//         _genericSets = GetType()
//             .GetProperties()
//             .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(ISet)))
//             .ToDictionary(
//                 n => n.PropertyType,
//                 n =>
//                 {
//                     var constructor = n.PropertyType.GetConstructor(new[] {typeof(Context)});
//                     var set = constructor?.Invoke(new object[] { this }) as ISet;
//                     n.SetValue(this, set);
//                     return set;
//                 });
//     }
//
//     public virtual void SaveChanges(string transactionName)
//     {
//         if (_isChangesSaving || false == HasAnyChanges())
//             return;
//
//         try
//         {
//             _isChangesSaving = true;
//
//             ChangesSaving?.Invoke(this, new ChangesSavingArgs(transactionName));
//
//             foreach (var entityEntry in DeletedEntityStore.Values)
//             {
//                 var set = entityEntry.Set;
//                 set.RemoveFromSet(entityEntry.EntityObject);
//
//                 var key = set.GetKey(entityEntry.EntityObject);
//                     
//                 if (AddedEntityStore.ContainsKey(key))
//                     AddedEntityStore.Remove(key);
//
//                 if (ModifiedEntityStore.ContainsKey(key))
//                     ModifiedEntityStore.Remove(key);
//             }
//
//             DeletedEntityStore.Clear();
//
//             foreach (var entityEntry in AddedEntityStore.Values)
//             {
//                 var set = entityEntry.Set;
//
//                 set.AddToSet(entityEntry.EntityObject);
//
//                 var key = set.GetKey(entityEntry.EntityObject);
//
//                 if (ModifiedEntityStore.ContainsKey(key))
//                     ModifiedEntityStore.Remove(key);
//             }
//
//             AddedEntityStore.Clear();
//
//             foreach (var entityEntry in ModifiedEntityStore.Values)
//             {
//                 var set = entityEntry.Set;
//                 set.UpdateInSet(entityEntry.EntityObject);
//             }
//
//             ModifiedEntityStore.Clear();
//         }
//         finally
//         {
//             _isChangesSaving = false;
//         }
//     }
//
//     public void Reset()
//     {
//         _genericSets.ForEach(n => n.Value.Reset());
//     }
//
//     public Set<TEntity> Set<TEntity>() where TEntity : class
//     {
//         return (Set<TEntity>) (_genericSets.TryGetValue(typeof(Set<TEntity>), out var set) ? set : null /*new Set<TEntity>(this)*/);
//     }
//
//     public Set Set(Type type)
//     {
//         return (_genericSets.TryGetValue(typeof(Set<>).MakeGenericType(type), out var set) ? set : null) as Set;
//     }
//
//     public EntityEntry Entry<TEntity>(TEntity entity) where TEntity : class
//     {
//         var set = Set(entity.GetType());
//         var key = set.GetKey(entity);
//
//         if (DeletedEntityStore.TryGetValue(key, out var deletedEntry))
//         {
//             return deletedEntry;
//         }
//
//         else if (AddedEntityStore.TryGetValue(key, out var addedEntry))
//         {
//             return addedEntry;
//         }
//
//         else if (ModifiedEntityStore.TryGetValue(key, out var modifiedEntry))
//         {
//             return modifiedEntry;
//         }
//
//         else
//         {
//             return set.ExistInSet(key)
//                 ? new EntityEntry(entity, EntityState.Unchanged, set)
//                 : new EntityEntry(entity, EntityState.Detached, set);
//         }
//     }
// }