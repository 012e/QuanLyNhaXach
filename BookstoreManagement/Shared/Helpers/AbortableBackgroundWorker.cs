using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Shared.Helpers;
public class AbortableBackgroundWorker : BackgroundWorker
{

    private Thread workerThread;

    protected override void OnDoWork(DoWorkEventArgs e)
    {
        workerThread = Thread.CurrentThread;
        try
        {
            base.OnDoWork(e);
        }
        catch (ThreadAbortException)
        {
            e.Cancel = true; //We must set Cancel property to true!
            Thread.ResetAbort();
        }
    }


    public void Abort()
    {
        if (workerThread != null)
        {
            workerThread.Abort();
            workerThread = null;
        }
    }
}
