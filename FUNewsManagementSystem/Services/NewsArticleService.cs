using BusinessObjects;
using DataAccessObjects;
using DataAccessObjects.DAO;
using Repositories;
using Services.DTO;
using Services.DTO.Services.DTO;
using System;
using System.Collections.Generic;

namespace Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository newsArticleRepository;

        public NewsArticleService(INewsArticleRepository repository)
        {
            newsArticleRepository = repository;
        }

        // Keep existing methods as they are
        public void AddNewsArticle(NewsArticle newNewsArticle)
        {
            newsArticleRepository.AddNewsArticle(newNewsArticle);
        }

        public void SaveNewsArticle(NewsArticle newsArticle)
        {
            newsArticleRepository.SaveNewsArticle(newsArticle);
        }

        public void DeleteNewsArticle(NewsArticle newsArticle)
        {
            newsArticleRepository.DeleteNewsArticle(newsArticle);
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            newsArticleRepository.UpdateNewsArticle(newsArticle);
        }

        public List<NewsArticle> GetNewsArticles()
        {
            return newsArticleRepository.GetNewsArticles();
        }

        public NewsArticle GetNewsArticleById(string id)
        {
            return newsArticleRepository.GetNewsArticleById(id);
        }

        public bool NewsArticleExists(string id)
        {
            try
            {
                return newsArticleRepository.GetNewsArticleById(id) != null;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate)
        {
            return newsArticleRepository.GetNewsArticlesByPeriod(startDate, endDate);
        }

        public List<NewsArticle> GetNewsByCreator(short creatorId)
        {
            return newsArticleRepository.GetNewsByCreator(creatorId);
        }

        public List<NewsArticle> SearchNewsByKeyword(string keyword)
        {
            return newsArticleRepository.SearchNewsByKeyword(keyword);
        }

        public List<NewsArticle> GetActiveNewsArticles()
        {
            return newsArticleRepository.GetActiveNewsArticles();
        }

        public IQueryable<NewsArticle> GetNewsArticlesQueryable()
        {
            return newsArticleRepository.GetNewsArticlesQueryable();
        }

        // Enhanced methods with DTOs
        public async Task<NewsArticleResponseDto> AddNewsArticleAsync(NewsArticleRequestDto request)
        {
            var article = await ConvertFromRequestDto(request);
            await newsArticleRepository.AddNewsArticleAsync(article);

            var createdArticle = newsArticleRepository.GetNewsArticleById(article.NewsArticleId);
            return ConvertToResponseDto(createdArticle);
        }

        public async Task<NewsArticleResponseDto> UpdateNewsArticleAsync(string id, NewsArticleRequestDto request)
        {
            var existingArticle = newsArticleRepository.GetNewsArticleById(id);
            if (existingArticle == null)
            {
                throw new KeyNotFoundException($"NewsArticle with ID '{id}' not found.");
            }

            var updatedArticle = await ConvertFromRequestDto(request);
            updatedArticle.NewsArticleId = id;
            updatedArticle.CreatedDate = existingArticle.CreatedDate;
            updatedArticle.CreatedById = existingArticle.CreatedById;

            var result = await newsArticleRepository.UpdateNewsArticleAsync(updatedArticle);
            return ConvertToResponseDto(result);
        }

        public async Task<bool> DeleteNewsArticleAsync(string id)
        {
            try
            {
                var article = newsArticleRepository.GetNewsArticleById(id);
                newsArticleRepository.DeleteNewsArticle(article);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public NewsArticleResponseDto GetNewsArticleResponseDto(string id)
        {
            var article = newsArticleRepository.GetNewsArticleById(id);
            return ConvertToResponseDto(article);
        }

        // DTO Conversion
        public NewsArticleResponseDto ConvertToResponseDto(NewsArticle article)
        {
            return new NewsArticleResponseDto
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                Headline = article.Headline,
                CreatedDate = article.CreatedDate,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                CategoryId = article.CategoryId,
                CategoryName = article.Category?.CategoryName,
                NewsStatus = article.NewsStatus,
                CreatedById = article.CreatedById,
                CreatedByName = article.CreatedBy?.AccountName,
                UpdatedById = article.UpdatedById,
                ModifiedDate = article.ModifiedDate,
                Tags = article.NewsTags?.Select(nt => new TagResponseDto
                {
                    TagId = nt.Tag.TagId,
                    TagName = nt.Tag.TagName
                }).ToList()
            };
        }

        public async Task<NewsArticle> ConvertFromRequestDto(NewsArticleRequestDto request)
        {
            var article = new NewsArticle
            {
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus,
                CreatedById = (short)request.CreatedById,
                UpdatedById = (short)request.UpdatedById
            };

            // Handle tags
            var newsTags = new List<NewsTag>();

            // If tag names are provided
            if (request.TagNames != null && request.TagNames.Any())
            {
                var tags = TagDAO.GetOrCreateTags(request.TagNames);
                newsTags.AddRange(tags.Select(tag => new NewsTag
                {
                    TagId = tag.TagId,
                    Tag = tag
                }));
            }

            // If tag IDs are provided
            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    var tag = TagDAO.GetTagById(tagId);
                    if (tag != null)
                    {
                        newsTags.Add(new NewsTag
                        {
                            TagId = tagId,
                            Tag = tag
                        });
                    }
                }
            }

            article.NewsTags = newsTags;
            return article;
        }
    }
}
