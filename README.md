# Panteon.Sdk
Panteon.Sdk

![](https://github.com/PanteonProject/panteon-dashboard/blob/master/misc/path4141.png)  

**It uses [Schyntax](https://github.com/schyntax/schyntax) for defining  event schedules.**
> Schyntax is a domain-specific language for defining event schedules in a terse, but readable, format. For example, if you want something to run every five minutes, you could write min(*%5).


## Preview  

**Realtime Task**  
```csharp
 public class HelloTask: RealtimePanteonWorker {
 	
 	public HelloTask(ILogger logger, 
 	       IHelloTaskSettings taskSettings, 
 	       IPubSubClient pubSubClient)
 	    : base(logger, taskSettings, pubSubClient) {
           }

 	public override string Name = > "My-Hello-Task";

 	public override bool Init(bool autoRun) {
 		return Run((task, offset) = > DoSomething());
 	}

 	private void DoSomething() {
 		string message = $ "{Name} Hello {DateTime.Now}";

 		for (int i = 0; i < 1000000; i++) {
 			var tmp = i / 100000;

 			if (i % 100000 == 0) {
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
  public class SampleTask: PanteonWorker {
  	public SampleTask(ILogger logger, 
  		ISampleTaskSettings taskSettings)
  	: base(logger, taskSettings) {
  	}
  	public override string Name = > "Sample-Task";

  	public override bool Init(bool autoRun) {
  		return Run((task, offset) = >
  		  Console.WriteLine($ "Dummy Hello {DateTime.Now}"));
  	}
  }
```

_More coming soon (:_

## License
Code and documentation are available according to the *MIT* License (see [LICENSE](https://raw.githubusercontent.com/PanteonProject/Panteon.Sdk/master/LICENSE)).
