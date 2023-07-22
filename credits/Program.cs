using Microsoft.Azure.Management.Billing;
using Microsoft.Azure.Management.Consumption;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;


namespace credits
{
    public class elememnts { 
        
        public string name { get; set; }
        public double amount { set; get; }
    }
    public class Program
    {
        
        static void Main(string[] args)
        {
            string clientId = "9a2c5988-836c-45b5-b923-08f89bd901ed";
            string clientSecret = "-Kx8Q~CHJxaFi3A.9X6OPf_0GZn.RPcKyXTCGdnU";
            string tenantId = "766989ef-a36d-498b-bebf-a91f68145a41";
            string subscriptionId = "61a43c33-3c94-471a-a1d6-2426304f599b";


            var credentials = new AzureCredentialsFactory().FromServicePrincipal(
                clientId, clientSecret, tenantId,
                AzureEnvironment.AzureGlobalCloud
                );

            // Create the Consumption Management Client
            var consumptionClient = new ConsumptionManagementClient(credentials)
            {
                SubscriptionId = subscriptionId
            };

            // Get the current period
            var dateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
            var startDateTime = DateTime.UtcNow.AddDays(-30).ToString(dateTimeFormat);
            var endDateTime = DateTime.UtcNow.ToString(dateTimeFormat);

            // Get the usage details
            var costDetails = consumptionClient.UsageDetails.List(
                expand: "properties/meterDetails",
                filter: $"properties/usageStart ge '{startDateTime}' and properties/usageEnd le '{endDateTime}'"
            );

            //getting jsonresponse

           /* string jsonString = JsonSerializer.Serialize(costDetails, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(jsonString);*/


            var total_ammount = costDetails
                .Where(c => c.Product != "")
                .Select(a => a.PretaxCost);


            var resource_totalcost = costDetails
                .Where(p => p.Product != "")
                .GroupBy(g => g.ConsumedService)    
                .Select(a => new elememnts()
                {
                    name = a.First().ConsumedService,
                    amount = (double)a.Sum(g=>g.PretaxCost)
                });



            var location_totalcost = costDetails
               .Where(p => p.Product != "")
               .GroupBy(g => g.InstanceLocation)
               .Select(a => new elememnts()
               {
                   name = a.First().InstanceLocation,
                   amount = (double) a.Sum(g => g.PretaxCost)
               });

            List<IEnumerable<elememnts>> res = new List<IEnumerable<elememnts>>();
            res.Add(resource_totalcost);
            res.Add(location_totalcost);

            foreach (var r in res) { 
                Console.WriteLine($"---------------------------{r}----------------------------");
                var total_cost = 0.0;
                foreach (var c in r) {
                    Console.WriteLine($"{c.name},{c.amount}");
                    total_cost += c.amount;
                }
                Console.WriteLine($"Total cost: {total_cost:f2}");
                Console.WriteLine();

            }




            /*  Console.WriteLine($"total cunsumed credits: {resource_totalcost.Sum(a => a.amount):f2}");

              double cost = (double)total_ammount.Sum();
              double cost2 = 0;
  */
            // Display the cost details



            /* foreach (var costDetail in costDetails)
             {
                 if (costDetail.Product != "")
                 {
                     Console.WriteLine($"Resource: {costDetail.Name}");
                     Console.WriteLine($"Product: {costDetail.Product}");
                     Console.WriteLine($"ConsumedService: {costDetail.ConsumedService}");
                     Console.WriteLine($"CostCenter: {costDetail.CostCenter}");
                     Console.WriteLine($"InvoiceId: {costDetail.InvoiceId}");
                     Console.WriteLine($"SubscriptionGuid: {costDetail.SubscriptionGuid}");
                     Console.WriteLine($"AdditionalProperties: {costDetail.AdditionalProperties}");
                     Console.WriteLine($"UsageQuantity: {costDetail.UsageQuantity}");
                     Console.WriteLine($"Usage Start: {costDetail.UsageStart}");
                     Console.WriteLine($"Usage End: {costDetail.UsageEnd}");
                     Console.WriteLine($"Cost: {costDetail.PretaxCost}");
                     //cost2 += (double)costDetail.PretaxCost;
                     Console.WriteLine();
                 }

             }

             Console.WriteLine("Total cost:" + $"{(int)cost}");
             Console.WriteLine("Total cost:" + $"{(int)cost2}");*/

        }
    }
}