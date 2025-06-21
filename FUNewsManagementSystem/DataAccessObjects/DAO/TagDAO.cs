using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class TagDAO
    {
        public static List<Tag> GetAllTags()
        {
            using var context = new FunewsManagementContext();
            return context.Tags.ToList();
        }

        public static Tag GetTagById(int id)
        {
            using var context = new FunewsManagementContext();
            return context.Tags.FirstOrDefault(t => t.TagId == id);
        }

        public static Tag GetTagByName(string name)
        {
            using var context = new FunewsManagementContext();
            return context.Tags.FirstOrDefault(t => t.TagName == name);
        }

        public static async Task<Tag> CreateTagAsync(string tagName)
        {
            using var context = new FunewsManagementContext();

            var existingTag = await context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
            if (existingTag != null)
                return existingTag;

            var newTag = new Tag { TagName = tagName };
            await context.Tags.AddAsync(newTag);
            await context.SaveChangesAsync();
            return newTag;
        }

        public static List<Tag> GetOrCreateTags(List<string> tagNames)
        {
            using var context = new FunewsManagementContext();
            var tags = new List<Tag>();

            foreach (var tagName in tagNames)
            {
                var existingTag = context.Tags.FirstOrDefault(t => t.TagName == tagName);
                if (existingTag != null)
                {
                    tags.Add(existingTag);
                }
                else
                {
                    var newTag = new Tag { TagName = tagName };
                    context.Tags.Add(newTag);
                    context.SaveChanges();
                    tags.Add(newTag);
                }
            }

            return tags;
        }
    }
}