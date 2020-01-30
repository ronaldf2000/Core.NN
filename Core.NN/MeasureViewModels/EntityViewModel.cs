using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Classes;

namespace Core.NN.MeasureViewModels
{
    public class EntityViewModel
    {
        //all text and int fields, but can be copied from craft
        //and list of attribute ViewModels
        //for classification, status, variation we only need the reference of these classes.

        public string classReference { get; set; }
        public string variationReference { get; set; }
        public string statusReference { get; set; }
        public string reference { get; set; }
        public string name { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public List<AttributeViewModel> attributes { get; set; }
        public List<LinkViewModel> children { get; set; }

        public EntityViewModel(Entity entity, int level, int maxLevel)
        {
            entity.LoadLinks();
            classReference = entity.Classification.Reference;
            variationReference = entity.Variation.Reference;
            statusReference = entity.Status.Reference;
            reference = entity.Reference;
            name = entity.Name;
            startDate = entity.StartDate;
            endDate = entity.EndDate;
            attributes = new List<AttributeViewModel>();
            foreach (EntityAttribute ea in entity.Attributes)
            {
                attributes.Add(new AttributeViewModel(ea));
            }
            if(level < maxLevel)
            {
                children = new List<LinkViewModel>();
                foreach (Link link in entity.Links)
                {
                    children.Add(new LinkViewModel(link, level, maxLevel));
                }
            }
        }

    }
}
