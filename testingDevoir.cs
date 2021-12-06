using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace TestingPerso
{
    public class testingDevoir : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region declaration params
            IPluginExecutionContext context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            ITracingService tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
            IOrganizationServiceFactory serviceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            #endregion
            try
            {
                switch (context.MessageName.ToLower())
                {
                    case "create":
                        if (context.Stage == 20)
                        {
                            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                            {
                                Entity entity = context.InputParameters["Target"] as Entity;
                                EntityReference entityReference = context.InputParameters["Target"] as EntityReference;
                                //Declarations
                                decimal noteEtudiant = (decimal)entity["acs_noteetudiant"];
                                //EntityReference etudiantt = (EntityReference)entity.Attributes["acs_etudiant"];
                                EntityReference etudiantt = entity.GetAttributeValue<EntityReference>("acs_etudiant");
                                EntityReference evaluation = entity.GetAttributeValue<EntityReference>("acs_evaluation");

                                //Query Expression for etudiant
                                QueryExpression query = new QueryExpression("acs_devoir");
                                query.ColumnSet.AddColumns("acs_noteetudiant", "acs_etudiant", "acs_evaluation");
                                query.Criteria = new FilterExpression();
                                FilterExpression filter = query.Criteria.AddFilter(LogicalOperator.And);
                                filter.AddCondition("acs_etudiant", ConditionOperator.Equal, etudiantt.Id);
                                filter.AddCondition("acs_evaluation", ConditionOperator.Equal, evaluation.Id);
                                EntityCollection results = service.RetrieveMultiple(query);

                                tracingService.Trace(results.Entities.Count.ToString());
                                tracingService.Trace("Note actuelle: " + noteEtudiant);
                                tracingService.Trace("etudiant.id.tostring: " + etudiantt.Id);
                                tracingService.Trace("etudiant acs_etudiant: " + entity["acs_etudiant"]);

                                //Query Expression for Eval
                                QueryExpression query1 = new QueryExpression("acs_devoir");
                                query1.ColumnSet.AddColumns("acs_meilleurenote", "acs_evaluation");
                                query1.Criteria.AddCondition("acs_evaluation", ConditionOperator.Equal, evaluation.Id);
                                EntityCollection result1 = service.RetrieveMultiple(query1);

                                if (results.Entities.Count == 0)
                                {
                                    tracingService.Trace("count = 1");
                                    entity["acs_notemoyenne"] = noteEtudiant;
                                }
                                if (results.Entities.Count >= 1)
                                {
                                    tracingService.Trace("count > 1");

                                    decimal totalNote = 0;
                                    foreach (Entity item in results.Entities)
                                    {
                                        if (item.Contains("acs_noteetudiant"))
                                        {
                                            decimal notes = item.GetAttributeValue<decimal>("acs_noteetudiant");
                                            totalNote += notes;
                                            tracingService.Trace("item notes =>" + item.GetAttributeValue<decimal>("acs_noteetudiant"));
                                        }
                                        else
                                        {
                                            tracingService.Trace("No acs_noteetudiant value found :(");
                                        }
                                    }
                                    totalNote = totalNote + noteEtudiant;


                                    tracingService.Trace("tot note " + totalNote);
                                    decimal moyenne = totalNote / (results.Entities.Count + 1);
                                    tracingService.Trace("moyenne " + moyenne);
                                    entity["acs_notemoyenne"] = moyenne;
                                }

                                if (result1.Entities.Count == 0)
                                {
                                    entity["acs_meilleurenote"] = noteEtudiant;
                                }
                                if (result1.Entities.Count >= 1)
                                {
                                    decimal notes = 0;
                                    foreach (Entity item in result1.Entities)
                                    {
                                        if (item.Contains("acs_noteetudiant"))
                                        {
                                            notes = item.GetAttributeValue<decimal>("acs_noteetudiant");
                                        }
                                    }
                                    entity["acs_meilleurenote"] = Math.Max(notes ,noteEtudiant);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {
                throw new InvalidPluginExecutionException();
            }

        }
    }
}
