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

        public static void AddTag(Tag tag)
        {
            using var context = new FunewsManagementContext();
            context.Tags.Add(tag);
            context.SaveChanges();
        }

        public static void UpdateTag(Tag tag)
        {
            using var context = new FunewsManagementContext();
            context.Tags.Update(tag);
            context.SaveChanges();
        }

        public static void DeleteTag(int tagId)
        {
            using var context = new FunewsManagementContext();
            var tag = context.Tags
                             .Include(t => t.NewsTags)
                             .FirstOrDefault(t => t.TagId == tagId);

            if (tag == null)
                throw new Exception($"Tag with ID {tagId} not found.");

            if (tag.NewsTags != null && tag.NewsTags.Any())
                throw new InvalidOperationException("Cannot delete tag, it is associated with a news article.");

            context.Tags.Remove(tag);
            context.SaveChanges();
        }
    }
}