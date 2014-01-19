using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Weblog.DynamicDomainObject
{
    public class DynamicDomainObject : IDynamicMetaObjectProvider
    {
        private Dictionary<String, Object> _internalPropertyStorage;

        public DynamicDomainObject()
        {
            _internalPropertyStorage = new Dictionary<String, Object>();
        }

        public Object SetProperty(String key, Object value)
        {
            if (_internalPropertyStorage.ContainsKey(key))
            {
                _internalPropertyStorage[key] = value;
            }
            else
            {
                _internalPropertyStorage.Add(key, value);
            }
            return value;
        }

        public Object GetProperty(String key)
        {
            if (_internalPropertyStorage.ContainsKey(key))
            {
                return _internalPropertyStorage[key];
            }
            return null;
        }

        public void Dispose()
        {
            _internalPropertyStorage.Clear();
            _internalPropertyStorage = null;
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicDomainMetaObject(parameter, this, GetType());
        }

        private class DynamicDomainMetaObject : DynamicMetaObject
        {
            private Type _type;

            internal DynamicDomainMetaObject(
                Expression parameter, DynamicDomainObject value, Type type)
                : base(parameter, BindingRestrictions.Empty, value)
            {
                this._type = type;
            }

            public override DynamicMetaObject BindConvert(ConvertBinder binder)
            {
                var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);

                if (binder.ReturnType.IsAssignableFrom(_type))
                    return new DynamicMetaObject(Expression.Constant(Value), restrictions);
                else
                    return new DynamicMetaObject(Expression.Default(binder.ReturnType), restrictions);
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                var propertyInfo = _type.GetProperties().FirstOrDefault(p => p.Name == binder.Name);

                var self = Expression.Convert(Expression, LimitType);

                Expression target;

                if (propertyInfo == null)
                {
                    target = Expression.Call(
                        self,
                        typeof(DynamicDomainObject).GetMethod("GetProperty"),
                        new Expression[] { Expression.Constant(binder.Name) }
                        );
                    target = FixReturnType(binder, target);
                }
                else
                {
                    target = Expression.Property(self, propertyInfo);
                    target = FixReturnType(binder, target);
                }

                return new DynamicMetaObject(target, restrictions);
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                var propertyInfo = _type.GetProperties().FirstOrDefault(p => p.Name == binder.Name);

                var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                var self = Expression.Convert(Expression, LimitType);

                Expression setCall;

                if (propertyInfo == null)
                {
                    var argument = Expression.Convert(value.Expression, typeof(Object));

                    setCall = Expression.Call(self, typeof(DynamicDomainObject).GetMethod("SetProperty"),
                                              new Expression[] { Expression.Constant(binder.Name), argument });
                }
                else
                {
                    var argument = Expression.Convert(value.Expression, propertyInfo.PropertyType);

                    setCall = Expression.Call(self, propertyInfo.GetSetMethod(),
                                              new Expression[] { argument });
                }

                return new DynamicMetaObject(Expression.Block(setCall, Expression.Default(binder.ReturnType)), restrictions);
            }

            private static Expression FixReturnType(DynamicMetaObjectBinder binder, Expression target)
            {
                if (target.Type != binder.ReturnType)
                {
                    if (target.Type == typeof(void))
                    {
                        target = Expression.Block(target, Expression.Default(binder.ReturnType));
                    }
                    else if (binder.ReturnType == typeof(void))
                    {
                        target = Expression.Block(target, Expression.Empty());
                    }
                    else
                    {
                        target = Expression.Convert(target, binder.ReturnType);
                    }
                }
                return target;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _type.GetProperties().Select(p => p.Name);
            }
        }
    }
}
