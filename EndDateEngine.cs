using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Classes;

using NetJSON;

namespace Core.NN
{
    public class EndDateEngine : RunnableEngine
    {

        protected string SerializaMeasure(Entity entity)
        {
            NetJSONSettings formatsettings = new NetJSONSettings()
            {
                DateFormat = NetJSONDateFormat.ISO,
                TimeZoneFormat = NetJSONTimeZoneFormat.Utc,
                CaseSensitive = false
            };
            MeasureViewModels.EntityViewModel entityView = new MeasureViewModels.EntityViewModel(entity, 0, 3);
            string json = NetJSON.NetJSON.Serialize(entityView, formatsettings);
            return json;
        }

        private Entity GetCaseEntity(Entity entity)
        {
            string CaseType = entity.Attributes["msr_selection"].Value;
            if (!string.IsNullOrEmpty(CaseType))
            {
                string reference = entity.Attributes["msr_case_" + CaseType].Reference;
                if (!string.IsNullOrEmpty(reference))
                {
                    return Universe.GetEntityByReference(reference, Universe.Classifications[CaseType]);
                }
            }
            return null;
        }

        public override object Process(Entity en, object o)
        {
            Classification measureClass = Universe.Classifications["MSR"];
            Variation measureVariation = Universe.Variations["Schade en inkomen"];
            StatusCode measureOpenStatus = measureClass.StatusCodes["MeasureOpenStatus"];
            StatusCode measureProcessingStatus = measureClass.StatusCodes["MeasureProcessingStatus"];


            string eodToDay = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-mm-dd HH:mm:ss");

            using (Query q = new Query())
            {
                q.CommandText = @"SELECT DISTINCT e.Id as EntityId from Entities e WITH(NOLOCK)
                                    INNER JOIN Attributes a (NOLOCK) on e.Id = a.EntityID
                                    INNER JOIN AttributeTypes att (NOLOCK) on att.ID = a.TypeID
                                 WHERE 
                                    e.ClassID = @ClassID 
                                    AND e.VariationID = @VariationID 
                                    AND e.StatusId = @StatusID
                                    AND att.Reference = @EndDtAtt
                                    AND a.Value <= @CurrentDt";

                q.Parameter("@ClassID", measureClass.ID);
                q.Parameter("@VariationID", measureVariation.ID);
                q.Parameter("@StatusID", measureOpenStatus.ID);
                q.Parameter("@EndDtAtt", "enddt_ivr");
                q.Parameter("@CurrentDt", eodToDay);
                q.Select();

                while (!q.EOF)
                {
                    Entity entity = new Entity(q.GetGuid("id"));
                    entity.Status = measureProcessingStatus;
                    entity.SetAttribute("ivr", "False", true);
                    entity.Save();
                    entity.LoadLinks();

                    Fact f = new Fact();
                    f.Archetype = Universe.Classifications["registratiefeit"];
                    f.CaseID = GetCaseEntity(entity).ID;
                    f.EntityID = entity.ID;
                    f.Reference = entity.Reference;
                    f.Notes = SerializaMeasure(entity);
                    f.Save();
                    Universe.Context.Notify(Universe.Translate("MEASURE.EXECUTE_SUCCES"));
                    entity.Status = measureOpenStatus;
                    entity.Save();

                    q.MoveNext();
                }

                return o;
            }
        }
    }
}
 