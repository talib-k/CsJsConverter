using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CsJsConversion.BaseClassMembers
{
    public class EnumInfo<T> where T : struct, IConvertible
    {
        public virtual IHtmlString GetText(T @enum)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            return new HtmlString(@enum.ToString(CultureInfo.InvariantCulture));
        }

        public virtual IHtmlString GetValue(T @enum)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            var val = Convert.ChangeType(@enum, @enum.GetTypeCode());
            return new HtmlString(Convert.ToString(val));
        }
    }
}
