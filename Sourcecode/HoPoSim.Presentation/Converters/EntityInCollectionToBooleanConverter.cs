using HoPoSim.Data.Domain;
using System;
using System.Collections;
using System.Linq;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
	public class EntityInCollectionToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() == 2)
            {
                var entity = values[0] as IEntity;
                var collection = values[1] as IEnumerable;

                return entity == null || 
                    collection == null || 
                    collection.Cast<IEntity>().Select(e => e.Id).Contains(entity.Id);
            }
            else
                return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
