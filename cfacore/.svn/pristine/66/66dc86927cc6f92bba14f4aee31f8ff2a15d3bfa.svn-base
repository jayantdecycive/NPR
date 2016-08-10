using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.shared.domain._base
{
    public interface IRange<T,Dx>
    {
        T Start { get; set; }
        T End { get; set; }
        /*
         * Setting Span should not save a value. Instead, it should overwrite the End property.
         * */
        Dx Span { get; set; }        
    }
}
