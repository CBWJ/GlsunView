using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.Domain;

namespace GlsunView.Infrastructure.Abstract
{
    public interface IDeviceLogger
    {
        void RecordConfig(User user, MachineFrame frame, List<DeviceOperationLog> logs);
    }
}
