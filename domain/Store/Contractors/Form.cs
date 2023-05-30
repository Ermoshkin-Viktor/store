using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Contractors
{
    public class Form
    {
        public string UniqueCode { get; }//код сервиса

        public int OrderId { get; }

        public int Step { get; }//шаг

        //Способ выяснить-последний ли это экран?(флаг)
        public bool IsFinal { get; }

        public IReadOnlyList<Field> Fields { get; }

        public Form(string uniqueCode, int orderId, int step, bool isFinal, IReadOnlyList<Field> fields)
        {
            if(string.IsNullOrWhiteSpace(uniqueCode))
                throw new ArgumentNullException(nameof(uniqueCode));

            if(step < 1)
                throw new ArgumentOutOfRangeException(nameof(step));

            if(fields == null)
                throw new ArgumentNullException(nameof(fields));

            UniqueCode = uniqueCode ;
            OrderId = orderId;
            Step = step;
            IsFinal = isFinal;
            Fields = fields ;
        }
    }
}
