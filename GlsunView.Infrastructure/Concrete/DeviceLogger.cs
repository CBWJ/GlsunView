using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.Domain;
using GlsunView.Infrastructure.Abstract;

namespace GlsunView.Infrastructure.Concrete
{
    public class DeviceLogger : IDeviceLogger
    {
        public void RecordConfig(User user, MachineFrame frame, List<DeviceOperationLog> logs)
        {
            using (var ctx = new GlsunViewEntities())
            {
                ctx.DeviceOperationLog.AddRange(logs);
            }
        }
    }
}
