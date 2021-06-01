using System.Reflection;

namespace Sks365.Ippica.Domain.Model
{
    public abstract class BaseDomainModel
    {
        /// <summary>
        /// Check if the object has all properties set to null, including the sub-objects
        /// </summary>
        public bool IsEmpty()
        {
            var isEmpty = true;
            foreach (PropertyInfo pi in GetType().GetProperties())
            {
                var value = pi.GetValue(this);

                isEmpty = (value == null) ||
                          (value != null && (value is BaseDomainModel) && (value as BaseDomainModel).IsEmpty());
                if (!isEmpty)
                    break;
            }

            return isEmpty;
        }
    }
}
