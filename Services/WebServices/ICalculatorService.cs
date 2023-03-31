using System.ServiceModel;

namespace poc_circuit_break_providers.Services.WebServices;

[ServiceContract]
public interface ICalculatorService
{
    [OperationContract(Action = "http://tempuri.org/Add", ReplyAction = "*")]
    Task<int> AddAsync(int intA, int intB);

    [OperationContract(Action = "http://tempuri.org/Subtract", ReplyAction = "*")]
    Task<int> SubtractAsync(int intA, int intB);

    [OperationContract(Action = "http://tempuri.org/Multiply", ReplyAction = "*")]
    Task<int> MultiplyAsync(int intA, int intB);

    [OperationContract(Action = "http://tempuri.org/Divide", ReplyAction = "*")]
    Task<int> DivideAsync(int intA, int intB);
}
