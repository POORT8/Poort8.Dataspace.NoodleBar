using FastEndpoints;

namespace Poort8.Dataspace.API;

public class EndpointsConfiguration
{
    public static void Filter(Config c, bool ishareEnabled)
    {
        c.Endpoints.Filter = ep =>
        {
            if (!ishareEnabled && ep.Routes?.Any(r => r.StartsWith("/api/ishare")) == true)
            {
                return false;
            }

            return true;
        };
    }

    public static void ConfigureProcessors(Config c)
    {
        c.Endpoints.Configurator = ep =>
        {
            ep.PreProcessor<GlobalRequestLogger>(Order.Before);
            ep.PostProcessor<GlobalResponseLogger>(Order.After);
        };
    }
}
