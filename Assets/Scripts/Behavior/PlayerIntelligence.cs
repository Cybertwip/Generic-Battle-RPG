using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class PlayerIntelligence : Intelligence
{
    protected bool DefendWindow { get; set; }
    public void OpenDefendWindow()
    {
        DefendWindow = true;
    }
    public abstract void TakeDamage(int amount);
    public void CloseDefendWindow()
    {
        DefendWindow = false;
    }
}