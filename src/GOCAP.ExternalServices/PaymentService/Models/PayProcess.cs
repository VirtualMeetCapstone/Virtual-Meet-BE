
namespace GOCAP.ExternalServices;

public class PayProcess
{
    private readonly PayOS _payOS;
    public PayProcess(PayOS payOS)
    {
        _payOS = payOS;
    }
}
