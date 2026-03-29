using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection(); 
var channel = connection.CreateModel();    

channel.QueueDeclare("notification", false, false, false, null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) => {
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"Received: {message}");
};

// Start the consumer
channel.BasicConsume("notification", true, consumer);

Console.WriteLine("SUCCESFULLY CONNECTED! Waiting for messages... jouhari");
Console.ReadLine();