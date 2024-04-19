using StreamingTitles.Data.Helper;
using StreamingTitles.Data.Model;
using System.Xml;

namespace StreamingTitles.Api
{
    public class Seed
    {
        private readonly TitlesContext _ctx;
        public Seed(TitlesContext context)
        {
            _ctx = context;
        }
        public void SeedDataContext()
        {
            if (!_ctx.Collection.Any() && !_ctx.Categories.Any() && !_ctx.Platforms.Any())
            {
                var titlesData = new List<TitleCategory>()
                {
                    new TitleCategory()
                    {
                        Title = new Title()
                        {
                            TitlePlatform = new List<TitlePlatform>()
                            {
                                new TitlePlatform()
                                {
                                    Platform = new Platform()
                                    {
                                        Name = "Netflix"
                                    }
                                }
                            },
                            Type = "Movie",
                            TitleName = "The Shawshank Redemption",
                            Country = "United States",
                            Release_Year = 1994
                        },
                        Category = new Category()
                        {
                            Name = "Drama"
                        }
                    },
                    new TitleCategory()
                    {
                        Title = new Title()
                        {
                            TitlePlatform = new List<TitlePlatform>()
                            {
                                new TitlePlatform()
                                {
                                    Platform = new Platform()
                                    {
                                        Name = "Hulu"
                                    }
                                }
                            },
                            Type = "Movie",
                            TitleName = "The Godfather",
                            Country = "United States",
                            Release_Year = 1972
                        },
                        Category = new Category()
                        {
                            Name = "Action"
                        }
                    }

                };
                _ctx.TitleCategories.AddRange(titlesData);
                _ctx.SaveChanges();
            }

        }
        public void SeedDataFromXML(string path)
        {
            var xmlReader = new XMLReader(path);
            var returndata = new List<Title>();
            var nodes = xmlReader.GetNodes();
            foreach (XmlNode node in nodes)
            {
                Console.WriteLine("Type: " + node["type"].InnerText);
                Console.WriteLine("Title: " + node["title"].InnerText);
                Console.WriteLine("Director: " + node["director"].InnerText);
                Console.WriteLine("Country: " + node["country"].InnerText);
                Console.WriteLine("Date Added: " + node["date_added"].InnerText);
                Console.WriteLine("Release Year: " + node["release_year"].InnerText);
                Console.WriteLine("Listed In: " + node["listed_in"].InnerText);


                var titleCategory = new List<TitleCategory>();
                var genres = node["listed_in"].InnerText.Split(", ");
                var netflixPlatform = _ctx.Platforms.FirstOrDefault(p => p.Name == "Netflix") ?? new Platform()
                {
                    Name = "Netflix"
                };
                foreach (var genre in genres)
                {
                    titleCategory.Add(new TitleCategory()
                    {
                        Category = _ctx.Categories.FirstOrDefault(c => c.Name == genre) ?? new Category()
                        {
                            Name = genre
                        }

                    });
                }
                var Title = new Title()
                {
                    TitlePlatform = new List<TitlePlatform>()
                    {
                        new TitlePlatform()
                        {
                            Platform = netflixPlatform
                        }
                    },
                    Type = node["type"].InnerText,
                    TitleName = node["title"].InnerText,
                    Country = node["country"].InnerText,
                    Release_Year = int.Parse(node["release_year"].InnerText),
                    TitleCategory = titleCategory
                };
                returndata.Add(Title);
                break;

            }
            _ctx.Collection.AddRange(returndata);
            _ctx.SaveChanges();

        }
    }
}
