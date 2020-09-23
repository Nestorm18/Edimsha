using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edimsha.Edition.Editor
{

    private static int id = 0;
    private static List<Resolution> resolutionList;

    public ResolutionService()
    {
        resolutionList = new List<Resolution>();
        Add(new Resolution { Id = 0, Width = 123, Height = 123 });
        Add(new Resolution { Id = 1, Width = 456, Height = 456 });
        Add(new Resolution { Id = 2, Width = 789, Height = 789 });
    }

    public List<Resolution> GetAll() { return resolutionList; }

    public bool Add(Resolution resolution)
    {
        if (resolution.Width <= 0 || resolution.Height <= 0)
            throw new ArgumentException("Width or Height must be greater than 0");

        resolution.Id = id++;
        resolutionList.Add(resolution);

        return true;
    }

    public bool Delete(int id)
    {
        for (int i = 0; i < resolutionList.Count; i++)
            if (resolutionList[i].Id == id)
                return true;
        return false;
    }

}

