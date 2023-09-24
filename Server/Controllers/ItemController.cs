using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly MarketDbContext _db;
        private readonly IDistributedCache _distributedCache;
        public ItemController(MarketDbContext db, IDistributedCache distributedCache)
        {
            this._db = db;
            this._distributedCache = distributedCache;
        }

        // GET: /Item
        [HttpGet]
        public async Task<List<Item>> GetItems()
        {
            return await _db.Items.ToListAsync();
        }

        // GET: /Item/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            string key = $"{id}";
            string? cacheMember = await _distributedCache.GetStringAsync(key);
            Item? item;
            if (string.IsNullOrEmpty(cacheMember))
            {
                item = await _db.Items.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                await _distributedCache.SetStringAsync(key,JsonConvert.SerializeObject(item));
                return item;
            }
            item=JsonConvert.DeserializeObject<Item>(cacheMember);
            return item;
        }

        // POST: /Item
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] Item item)
        {
            _db.Items.Add(item);
            await _db.SaveChangesAsync();
            return item.Id;
        }

        // PUT: /Item/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Item newItem)
        {
            if (id != newItem.Id)
            {
                return BadRequest();
            }
            var item = await _db.Items.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (item == null)
            {
                throw new Exception("Not Found");
            }
            item.Name = newItem.Name;
            item.Price = newItem.Price;
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Item/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Items.FindAsync(id);
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}