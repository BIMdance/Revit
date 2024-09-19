// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
//
// namespace CoLa.BimEd.Logic.SDK.DataAccess.Context;
//
// public abstract class Set : ISet
// {
//     protected Set(Type type)
//     {
//         Type = type;
//     }
//
//     public Type Type { get; }
//     public abstract void Reset();
//     protected internal abstract void AddToSet(object entity);
//     protected internal abstract bool ExistInSet(object key);
//     protected internal abstract void RemoveFromSet(object entity);
//     protected internal abstract void UpdateInSet(object entity);
//     protected internal abstract object GetKey(object entity);
// }
//
// public class Set<TEntity> : Set, ISet<TEntity>
//     where TEntity : class
// {
//     private readonly Context _context;
//     private readonly Dictionary<object, TEntity> _entities;
//     private readonly PropertyInfo _keyProperty;
//
//     public Set(Context context) : base(typeof(TEntity))
//     {
//         _context = context;
//
//         _entities = new Dictionary<object, TEntity>();
//
//         var properties = typeof(TEntity).GetProperties();
//
//         _keyProperty =
//             properties.FirstOrDefault(n => n.PropertyType == typeof(Guid) && n.Name == "Guid") ??
//             properties.FirstOrDefault(n => n.PropertyType == typeof(Guid) && n.Name == "Id") ??
//             properties.FirstOrDefault(n => n.Name == "Id") ??
//             properties.FirstOrDefault(n => n.Name == "Key") ??
//             properties.FirstOrDefault(n => n.PropertyType == typeof(Guid));
//     }
//
//     public bool IsInitialized { get; set; }
//
//     public void Initialize(IEnumerable<TEntity> entities)
//     {
//         if (IsInitialized)
//             throw new InvalidOperationException($"{nameof(Set)}<{typeof(TEntity).Name}> already was initialized.");
//             
//         foreach (var entity in entities)
//         {
//             object key = default;
//
//             try
//             {
//                 key = GetKey(entity);
//                 _entities.Add(key, entity);
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine($@"key: {key}");
//                 Console.WriteLine(e);
//                 // throw;
//             }
//         }
//
//         IsInitialized = true;
//     }
//
//     public override void Reset()
//     {
//         _entities.Clear();
//         IsInitialized = false;
//     }
//         
//     public IEnumerator<TEntity> GetEnumerator() => _entities.Values.GetEnumerator();
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//
//     public TEntity Add(TEntity entity)
//     {
//         if (entity == null)
//             throw new ArgumentNullException($"{nameof(entity)}");
//
//         var key = GetKey(entity);
//
//         if (!_context.AddedEntityStore.ContainsKey(key) &&
//             !_context.DeletedEntityStore.ContainsKey(key))
//             _context.AddedEntityStore.Add(key, new EntityEntry(entity, EntityState.Added, this));
//
//         return entity;
//     }
//
//     public TEntity Find(object key)
//     {
//         return _entities.TryGetValue(key, out var entity) ? entity : null;
//     }
//
//     public virtual TEntity Create()
//     {
//         throw new NotImplementedException();
//     }
//
//     public TEntity Remove(TEntity entity)
//     {
//         if (entity == null)
//             throw new ArgumentNullException($"{nameof(entity)}");
//
//         var key = GetKey(entity);
//
//         if (!_context.DeletedEntityStore.ContainsKey(key))
//             _context.DeletedEntityStore.Add(key, new EntityEntry(entity, EntityState.Deleted, this));
//             
//         return entity;
//     }
//
//     public TEntity Update(TEntity entity)
//     {
//         if (entity == null)
//             throw new ArgumentNullException($"{nameof(entity)}");
//             
//         var key = GetKey(entity);
//
//         if (!_context.AddedEntityStore.ContainsKey(key) &&
//             !_context.DeletedEntityStore.ContainsKey(key) &&
//             !_context.ModifiedEntityStore.ContainsKey(key))
//             _context.ModifiedEntityStore.Add(key, new EntityEntry(entity, EntityState.Modified, this));
//             
//         return entity;
//     }
//
//     public IQueryable<TEntity> Query()
//     {
//         return _entities.Values.AsQueryable();
//     }
//
//     protected internal override void AddToSet(object entity)
//     {
//         if (entity is not TEntity value)
//             throw new InvalidCastException($"{entity} ({entity.GetType().FullName}) is not a {typeof(TEntity).FullName}.");
//
//         var key = GetKey(entity);
//
//         try
//         {
//             if (_entities.ContainsKey(key))
//                 _entities[key] = value;
//             else
//                 _entities.Add(key, value);
//         }
//         catch (Exception)
//         {
//             Console.WriteLine($"\n\t entity: {entity}\n\t key: {key}");
//             throw;
//         }
//     }
//
//     protected internal override bool ExistInSet(object key)
//     {
//         return _entities.ContainsKey(key);
//     }
//
//     protected internal override void UpdateInSet(object entity)
//     {
//             
//     }
//
//     protected internal override void RemoveFromSet(object entity)
//     {
//         var key = GetKey(entity);
//         _entities.Remove(key);
//     }
//
//     protected internal override object GetKey(object entity)
//     {
//         return _keyProperty?.GetValue(entity) ?? entity.GetHashCode();
//     }
// }