using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteController.Models
{
    public class TvChannels
    {
        public string Id { get; set; }
        public int Zap { get; set; }
        public string Name { get; set; }
    }


    public class CategoriesList
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class CategoriesSets
    {
        public List<string> all { get; set; }
        public List<string> ppv { get; set; }
        public List<string> vod { get; set; }
        public List<string> kids { get; set; }
        public List<string> news { get; set; }
        public List<string> music { get; set; }
        public List<string> erotic { get; set; }
        public List<string> netia { get; set; }
    }

    public class PpvKsw33Prod
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Npvr
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Hbogo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Kinoplex
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Filmbox
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Tvnplayer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Ipla
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Abcvod
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Goon
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Tubafm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Tvpsport
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Tvnmeteo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Erowizja
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Pinkvision
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Netiacloud
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Premiumplus
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Netiatvshop
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Ninateka
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

    public class Apps
    {
        public PpvKsw33Prod ppv_ksw33_prod { get; set; }
        public Npvr npvr { get; set; }
        public Hbogo hbogo { get; set; }
        public Kinoplex kinoplex { get; set; }
        public Filmbox filmbox { get; set; }
        public Tvnplayer tvnplayer { get; set; }
        public Ipla ipla { get; set; }
        public Abcvod abcvod { get; set; }
        public Goon goon { get; set; }
        public Tubafm tubafm { get; set; }
        public Tvpsport tvpsport { get; set; }
        public Tvnmeteo tvnmeteo { get; set; }
        public Erowizja erowizja { get; set; }
        public Pinkvision pinkvision { get; set; }
        public Netiacloud netiacloud { get; set; }
        public Premiumplus premiumplus { get; set; }
        public Netiatvshop netiatvshop { get; set; }
        public Ninateka ninateka { get; set; }
    }

    public class RootObject
    {
        public List<CategoriesList> categories_list { get; set; }
        public CategoriesSets categories_sets { get; set; }
        public Apps apps { get; set; }
    }

    public class NetiaApp
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GridImg_url { get; set; }
        public string ListImg_url { get; set; }
        public bool isActive { get; set; }
    }

}
