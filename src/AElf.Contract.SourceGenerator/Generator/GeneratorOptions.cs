namespace ContractGenerator;

public class GeneratorOptions
{
    public bool InternalAccess { get; set; }
    public bool GenerateContract { get; set; }
    public bool GenerateReference { get; set; }
    public bool GenerateStub { get; set; }
    public bool GenerateEvent { get; set; }

    public bool GenerateEventOnly => GenerateEvent & !GenerateContract & !GenerateReference & !GenerateStub;
    public bool GenerateContainer => GenerateContract | GenerateReference | GenerateStub;
}
