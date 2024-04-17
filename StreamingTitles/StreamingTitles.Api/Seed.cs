using StreamingTitles.Data.Model;

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
                            Director = "Frank Darabont",
                            Cast = "Tim Robbins, Morgan Freeman, Bob Gunton",
                            Country = "United States",
                            Date_Added = new DateTime(2021, 1, 1),
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
                            Director = "Francis Ford Coppola",
                            Cast = "Marlon Brando, Al Pacino, James Caan",
                            Country = "United States",
                            Date_Added = new DateTime(2021, 1, 1),
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
    }
}
