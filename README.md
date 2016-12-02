# SingularityORM
Micro ORM for .NET 

Singularity ORM is a tiny and simply to use, object relational mapping framework for .NET environment, tailored to be used especially in projects responsible for exchange data with open source systems without their own API.

It's an entirely new approach towards micro ORM idea and due to that, might be completely separate from SQL queries, which are often required in the others similar projects.

Singularity.ORM is currently in the testing phase, and no giving any warranty.

#How to use

### 1. Connection

##### Directly in source code
```c#
SQLprovider Context = new SQLprovider(new ProviderCredentials() { 
              Server = "127.0.0.1",
              Port = 3306,
              User = "root",
              Password = "",
              Database = "testORM",
              Collation = Singularity.ORM.Enum.Collation.UTF8            
            });
```

##### Using sections in App.config.xml / Web.config.xml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="SingularityProvider" requirePermission="false"
             type="Singularity.ORM.SingularityProviderSection,Singularity.ORM"/>
  </configSections>
  <SingularityProvider 
        ServerAddress="localhost"  
        PortNumber="3306"
        UserName ="root"
        Password =""
        Database ="testORM"
        Collation = "UTF8"
        />
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
    </startup> 
</configuration>
```
### 2. Create new instance of an order object

```c#
/// Open a provider Facade
            using (SQLprovider proxy = new SQLprovider()) {
                /// SQL dump (if required)
                proxy.OnQueryAction += new OnQueryEventHandler(proxy_OnQueryAction);
                /// New trasanction
                using (ISqlTransaction trans = proxy.BeginTransaction(System.Data.IsolationLevel.Unspecified)) {
                    PaymentRepository repo = EntityRepository.GetInstance<PaymentRepository>(trans);
                    Order order = new Order()
                    {
                        OrderNumber = "9998/FV/2009/02",
                        OrderDate = DateTime.Now,
                        OrderJsonData = "{\"notifyUrl\":\"http://localhost/payupages\",\"customerIp\":\"127.0.0.1\",\"merchantPosId\":\"145227\",\"description\":\"klocki DUPLO\",\"currencyCode\":\"PLN\",\"totalAmount\":\"224900\",\"extOrderId\":\"3325844a-c6d8-4196-bb84-280d4052016b\",\"validityTime\":\"186400\",\"buyer\":{\"email\":\"jan.kowalski@vp.pl\",\"phone\":\"111-222-333\",\"firstName\":\"Jan\",\"lastName\":\"Kowalski\"},\"products\":[{\"name\":\"Zestaw Lego DUPLO z elektrycznym pociągiem\",\"unitPrice\":\"24999\",\"quantity\":\"1\"},{\"name\":\"Zestaw szyn\",\"unitPrice\":\"00\",\"quantity\":\"1\"}]}"
                    };
                    repo.Orders.Add(order);
                    trans.Commit();
                }
            } 
```

### 3. Searching data with specific conditions 

```c#
  Order.CurrentDebtChangedHandler(new EntityDelegate<Order>(ZmianaZadluzenia));

            /// Open a provider Facade
            using (SQLprovider proxy = new SQLprovider())
            {
                /// SQL dump (if required)
                proxy.OnQueryAction += new OnQueryEventHandler(proxy_OnQueryAction);
                /// New trasanction
                using (ISqlTransaction trans = proxy.BeginTransaction(System.Data.IsolationLevel.Unspecified))
                {
                    PaymentRepository repo = EntityRepository.GetInstance<PaymentRepository>(trans);
                    Order order = repo.Orders.GetFirst(SQLCondition.Empty);
                    order.CurrentDebt = 0;                    
                    trans.Commit();
                }
            }
            Console.ReadLine();       
        }
```

#Sample usage scenarios

Example of XML schema file contained entity structure description

![singularity-xml](https://cloud.githubusercontent.com/assets/8134988/20853866/60f4f290-b8ef-11e6-8a87-3789a37d30a5.png)

Example of Code generated entity schema class

![singularity-entity](https://cloud.githubusercontent.com/assets/8134988/20853864/5bd1add0-b8ef-11e6-8775-1ffb0e793481.png)

Example of use

![singularity-example](https://cloud.githubusercontent.com/assets/8134988/20853865/5eeb89be-b8ef-11e6-9f5d-463763e553d5.png)


