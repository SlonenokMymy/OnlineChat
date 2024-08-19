namespace OnlineChat.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OnlineChat.DataModels;

    [ApiController]
    [Route("[controller]")]
    public class ChannelsController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public ChannelsController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetChatHistory/{chatId}")]
        public async Task<IActionResult> GetChatHistory(int chatId)
        {
            var ChatHistory = await _context.ChatHistories.Where(x => x.ChatId == chatId).ToListAsync();

            if (ChatHistory == null)
            {
                return NotFound();
            }
            else return Ok(ChatHistory);
        }

        [HttpGet("GetAllChats/{ownerId}")]
        public async Task<IEnumerable<ChatInfo>> GetAllChats(int ownerId)
        {
            return await _context.ChatInfos.Where(x => x.OwnerId == ownerId).ToListAsync();
        }

        [HttpPost("SaveChat")]
        public async Task<ActionResult<ChatInfo>> PostChatInfo(ChatInfo chatInfo)
        {
            _context.ChatInfos.Add(chatInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(chatInfo), new { id = chatInfo.Id }, chatInfo);
        }

        [HttpPost("SaveMessage")]
        public async Task<ActionResult<ChatHistory>> PostChatHistory(ChatHistory chatHistory)
        {
            _context.ChatHistories.Add(chatHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(chatHistory), new { id = chatHistory.Id }, chatHistory);
        }

        [HttpDelete("DeleteMsg/{msgId}")]
        public async Task<IActionResult> DeleteMsg(int msgId)
        {
            var chatHistory = await _context.ChatHistories.FindAsync(msgId);
            if (chatHistory == null)
            {
                return NotFound();
            }

            _context.ChatHistories.Remove(chatHistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict(ex.ToString());
            }

            return NoContent();
        }

        [HttpDelete("DeleteChat/{chatId}")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            try
            {
                var chatHistory = await _context.ChatHistories.Where(x => x.ChatId == chatId).ToListAsync();
                if (chatHistory.Any())
                {
                    _context.ChatHistories.RemoveRange(chatHistory);
                }
                await _context.SaveChangesAsync();

                var chat = await _context.ChatInfos.FindAsync(chatId);

                _context.ChatInfos.Remove(chat);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict(ex.ToString());
            }
            

            return NoContent();
        }
    }
}
