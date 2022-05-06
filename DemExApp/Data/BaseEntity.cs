using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace DemExApp.Data
{
    public abstract class BaseEntity : INotifyPropertyChanged
    {
        [Key]
        public Guid UID { get; set; }

        [NotMapped]
        public object this[string property]
        {
            get
            {
                var propInfo = GetType().GetProperty(property);
                return propInfo.GetValue(this);
            }
            set 
            {
                var propInfo = GetType().GetProperty(property);
                propInfo.SetValue(this, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
