using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Models
{
    [Serializable]
    public abstract class NotifyPropertyChangedContext : INotifyPropertyChanged
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public T PreliminaryState<T>() where T : class, new()
        {
            T obj = new T();
            foreach (var property in obj.GetType().GetProperties())
            {
                if (!dictionary.ContainsKey(property.Name))
                    continue;
                obj.GetType().GetProperty(property.Name).SetValue(obj, dictionary[property.Name]);
            }
            return obj;
        }

        public NotifyPropertyChangedContext()
        {
            foreach (var property in this.GetType().GetProperties())
            {
                dictionary.TryAdd(property.Name, property.GetValue(this));
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            dictionary[propertyName] = this.GetType().GetProperty(propertyName).GetValue(this);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void CleanChanged()
        {
            dictionary.Clear();
            foreach (var property in this.GetType().GetProperties())
            {
                dictionary.TryAdd(property.Name, property.GetValue(this));
            }
        }
    }
}
