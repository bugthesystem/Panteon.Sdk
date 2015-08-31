# Panteon.Sdk
Panteon.Sdk

![](https://github.com/PanteonProject/panteon-dashboard/blob/master/misc/path4141.png)  


## Preview  

**Realtime Task**  
```csharp
//Task Exports
[Export(typeof(ITaskExports))]
    public class Exports : ITaskExports
    {
        public ContainerBuilder Builder
        {
            get
            {
                var builder = new ContainerBuilder();

                builder.RegisterModule<CoreModule>();

                builder.RegisterType<EnvironmentWrapper>().As<IEnvironmentWrapper>()
                    .SingleInstance();

                builder.RegisterType<JsonNetSerializer>().As<IJsonSerializer>()
                    .SingleInstance();

                builder.RegisterType<PubSubClient>().As<IPubSubClient>().SingleInstance();

                builder.Register(context => new HelloTaskConfigProvider(context.Resolve<ILogger>())
                    .ParseSettings())
                    .AsImplementedInterfaces().SingleInstance();

                builder.RegisterType<HelloTask>().As<IPanteonTask>();

                return builder;
            }
        }
    }

//Task Impl
 public class HelloTask : PanteonRealtimeTask, IDisposable
    {
        public HelloTask(ILogger logger, IHelloTaskSettings taskSettings, 
          IPubSubClient pubSubClient): base(logger, taskSettings, pubSubClient)
        {
        }
        
        public override string Name => "My-Hello-Task";

        public override bool Init(bool autoRun)
        {
            return Run((task, offset) => DoSomething());
        }

        private void DoSomething()
        {
            string message = $"{Name} Hello {DateTime.Now}";

            for (int i = 0; i < 1000000; i++)
            {
                var tmp = i / 100000;

                if (i % 100000 == 0)
                {
                    Progress(new ProgressMessage { 
                        Message = message,
                        Percent = 10m * tmp 
                    });
                }
            }

            Console.WriteLine(message);
        }
    }
```

**General Task**  
```csharp
//Task Exports
[Export(typeof(ITaskExports))]
    public class Exports : ITaskExports
    {
        public ContainerBuilder Builder
        {
            get
            {
                var builder = new ContainerBuilder();

                builder.RegisterModule<CoreModule>();

                builder.RegisterType<EnvironmentWrapper>().As<IEnvironmentWrapper>()
                    .SingleInstance();

                builder.RegisterType<JsonNetSerializer>().As<IJsonSerializer>()
                    .SingleInstance();

                builder.Register(context => new SampleTaskConfigProvider(context.Resolve<ILogger>())
                    .ParseSettings())
                    .AsImplementedInterfaces().SingleInstance();

                builder.RegisterType<SampleTask>().As<IPanteonTask>();

                return builder;
            }
        }
    }

//Task Impl
public class SampleTask : PanteonTask, IDisposable
    {
        public SampleTask(ILogger logger, ISampleTaskSettings taskSettings) 
            : base(logger, taskSettings)
        {
        }
        public override string Name => "My-Dummy-Task";

        public override bool Init(bool autoRun)
        {
            return Run((task, offset) => 
                Console.WriteLine($"Dummy Hello {DateTime.Now}"));
        }
    }
```

_**More coming soon (: **_

## License
Code and documentation are available according to the *MIT* License (see [LICENSE](https://raw.githubusercontent.com/PanteonProject/Panteon.Sdk/master/LICENSE)).
