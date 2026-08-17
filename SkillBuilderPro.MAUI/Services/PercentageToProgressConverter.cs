using System.Globalization;
namespace SkillBuilderPro.MAUI.Services;
public sealed class PercentageToProgressConverter:IValueConverter
{
 public object Convert(object? value,Type targetType,object? parameter,CultureInfo culture)=>value is int percentage?Math.Clamp(percentage/100d,0d,1d):0d;
 public object ConvertBack(object? value,Type targetType,object? parameter,CultureInfo culture)=>throw new NotSupportedException();
}
