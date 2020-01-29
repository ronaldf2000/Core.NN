using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Classes;

namespace Core.NN.MeasureViewModels
{
    public class LinkViewModel
    {
        public EntityViewModel child { get; set; }
        public string linkTypeReference { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string reference { get; set; }

        public LinkViewModel(Link link, int level,int maxLevel)
        {
            child = new EntityViewModel(link.Child, level + 1, maxLevel);
            linkTypeReference = link.LinkType.Reference;
            startDate = link.StartDate;
            endDate = link.EndDate;
            reference = link.Reference;
        }
    }
}
