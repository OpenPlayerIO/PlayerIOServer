using System;
using System.Linq;
using System.Reflection;

namespace OpenPlayerIO.PlayerIOServer.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary> Casts an object into the specified type. </summary>
        public static T Cast<T>(this object input)
        {
            var target = typeof(T);
            var output = Activator.CreateInstance(target, false);

            var targetMembers = target.GetMembers().ToList().Where(source => source.MemberType == MemberTypes.Property);
            var targetMembersInclusive = targetMembers.Where(memberInfo => targetMembers.Select(c => c.Name).ToList().Contains(memberInfo.Name));

            foreach (var memberInfo in targetMembersInclusive) {
                var propertyInfo = typeof(T).GetProperty(memberInfo.Name);
                var value = input.GetType().GetProperty(memberInfo.Name).GetValue(input, null);

                propertyInfo.SetValue(output, value, null);
            }

            return (T)output;
        }
    }
}