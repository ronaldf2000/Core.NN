using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Classes;

namespace Core.NN.MeasureViewModels
{
    public class AttributeViewModel
    {
        public string value { get; set; }
        public string attributeTypeReference { get; set; }
        public string reference { get; set; }

        public AttributeViewModel(EntityAttribute ea)
        {
            this.value = ea.Value;
            this.attributeTypeReference = ea.AttributeType.Reference;
            this.reference = ea.Reference;
        }
    }
}
