using System;
using System.Collections.Generic;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace SearchContactWorkflowExtension
{
    public class SearchContact : CodeActivity
    {
        //input arguments

        [RequiredArgument]
        [Input("Parameter 1 value")]
        public InArgument<string> Parameter1 { get; set; }

        [RequiredArgument]
        [Input("Parameter 1 name")]
        public InArgument<string> FirstParameterName { get; set; }

        [RequiredArgument]
        [Input("Parameter 2 value")]
        public InArgument<string> Parameter2 { get; set; }

        [RequiredArgument]
        [Input("Parameter 2 name")]
        public InArgument<string> SecondParameterName { get; set; }

        //output arguments

        [Output("Searched contact")]
        [ReferenceTarget("contact")]
        public OutArgument<List<EntityReference>> SearchedContacts { get; set; }

        [Output("Status")]
        public OutArgument<int> Status { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();

            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();        
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);

            string param1Value = Parameter1.Get(context);
            string param1Name = FirstParameterName.Get(context);

            string param2Value = Parameter2.Get(context);
            string param2Name = SecondParameterName.Get(context);

            int resultStatus;
            List<EntityReference> contactsReferences = new List<EntityReference>();

            try
            {
                QueryExpression query = new QueryExpression("contact");

                query.Criteria.AddCondition(new ConditionExpression($"{param1Name}", ConditionOperator.Equal, param1Value));
                query.Criteria.AddCondition(new ConditionExpression($"{param2Name}", ConditionOperator.Equal, param2Value));

                DataCollection<Entity> contacts = service.RetrieveMultiple(query).Entities;

                switch (contacts.Count)
                {
                    case 0:
                        resultStatus = 2;
                        break;
                    case 1:
                        resultStatus = 1;
                        break;
                    default:
                        resultStatus = 3;
                        break;
                }

                foreach (var contact in contacts)
                {
                    contactsReferences.Add(new EntityReference("contact", contact.Id));
                }

                SearchedContacts.Set(context, contactsReferences);
                Status.Set(context, resultStatus);             
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
