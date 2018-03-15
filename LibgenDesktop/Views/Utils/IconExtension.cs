using System;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace LibgenDesktop.Views.Utils
{
    public class IconExtension : MarkupExtension
    {
        private string source;

        public IconExtension()
        {
            Source = null;
            Size = 0;
        }

        public IconExtension(string source, int size)
        {
            Source = source;
            Size = size;
        }

        public int Size { get; set; }

        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                if (value.StartsWith("pack"))
                {
                    source = value;
                }
                else
                {
                    source = "pack://application:,,," + value;
                }
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            BitmapDecoder decoder = BitmapDecoder.Create(new Uri(Source), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);
            BitmapFrame result = decoder.Frames.SingleOrDefault(frame => frame.Width == Size);
            if (result == null)
            {
                result = decoder.Frames.OrderBy(frame => frame.Width).First();
            }
            return result;
        }
    }
}
