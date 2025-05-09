
using ZRoomLibrary;

namespace ZRoomBackendApi.Services
{
    public class BookingRotationService : BackgroundService
    {
        private readonly AvailableBookingsDatabaseUpdater _updater;

        public BookingRotationService(AvailableBookingsDatabaseUpdater updater)
        {
            _updater = updater;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;
                DateTime nextRunTime = now.Date.AddDays(1).AddHours(3);
                TimeSpan delay = nextRunTime - now;

                await Task.Delay(delay, stoppingToken);

                await _updater.RotateBookingsAsync();
            }
        }
    }
}
