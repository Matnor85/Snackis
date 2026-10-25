using Domain.Entities;

namespace Snackis.Components.Pages;

public partial class Bloglista
{
    private readonly IHttpClientFactory _clientFactory;
    public List<Topic> Topics { get; set; }

    public Bloglista(IHttpClientFactory clientFactory)
    {

        _clientFactory = clientFactory;

    }
}
