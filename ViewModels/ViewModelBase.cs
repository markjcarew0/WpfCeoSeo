﻿namespace CeoSeoViewModels
{
    using System.ComponentModel;

    /// <summary>
    ///     implement  base VIEWMODEL class
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        ///     The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="propertyName">
        ///     The property name.
        /// </param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}