

namespace ConfigService.Request
{
    public interface IRequestFactory
    {
        Request Create(int reqType, DC_RequestParam reqParam);
    }
}
