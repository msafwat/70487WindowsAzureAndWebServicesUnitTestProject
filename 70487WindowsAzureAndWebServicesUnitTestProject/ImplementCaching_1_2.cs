using System;
using NUnit.Framework;
using System.Runtime.Caching;

namespace WindowsAzureAndWebServicesUnitTestProject
{
    public class ImplementCaching_1_2
    {
        [TestCase("Cache1", 1)]
        [TestCase("Cache1", 2)]
        [TestCase("Cache1", 3)]
        public void CanCache(string key, int value)
        {
            // ARRANGE
            ObjectCache cache = MemoryCache.Default;
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(1))
            };

            // ACT
            cache.Remove(key);
            cache.Add(key, value, policy);
            int fetchedValue = (int)cache.Get(key);
            
            // ASSERT
            Assert.That(fetchedValue, Is.EqualTo(value), "Uh oh!");
        }

        [TestCase("Sliding1", 1)]
        [TestCase("Sliding2", 2)]
        [TestCase("Sliding3", 3)]
        public void TestSlidingExpiration(string key, int value)
        {
            // ARRANGE
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                SlidingExpiration = new TimeSpan(0, 0, 2)
            };
            cache.Set(key, value, policy);

            // ACT
            for (var i = 0; i < 22; i++)
            {
                System.Threading.Thread.Sleep(100);
                Assume.That(cache.Get(key), Is.EqualTo(value));
            }
            System.Threading.Thread.Sleep(2001);
            
            // ASSERT
            Assert.That(cache.Get(key), Is.Null, "Uh oh!");
        }

        [Test]
        public void BadCaching()
        {
            // ARRANGE
            System.Web.Caching.Cache myCache = new System.Web.Caching.Cache();
            // ACT
            // ASSERT
            Assert.Throws<NullReferenceException>(() => myCache.Insert("asdf", 1));
        }
    }
}
