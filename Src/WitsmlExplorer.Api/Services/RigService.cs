using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WitsmlExplorer.Api.Query;
using Witsml.ServiceReference;
using WitsmlExplorer.Api.Models;
using Serilog;

namespace WitsmlExplorer.Api.Services
{
    public interface IRigService
    {
        Task<IEnumerable<Rig>> GetRigs(string wellUid, string wellboreUid);
        Task<Rig> GetRig(string wellUid, string wellboreUid, string rigUid);
    }

    // ReSharper disable once UnusedMember.Global
    public class RigService : WitsmlService, IRigService
    {
        public RigService(IWitsmlClientProvider witsmlClientProvider) : base(witsmlClientProvider) { }

        public async Task<IEnumerable<Rig>> GetRigs(string wellUid, string wellboreUid)
        {
            var witsmlRigs = RigQueries.GetWitsmlRigByWellbore(wellUid, wellboreUid);
            var result = await WitsmlClient.GetFromStoreAsync(witsmlRigs, new OptionsIn(ReturnElements.IdOnly));
            return result.Rigs.Select(rig =>
                new Rig
                {
                    AirGap = rig.AirGap,
                    Name = rig.Name,
                    Owner = rig.Owner,
                    TypeRig = rig.TypeRig,
                    WellboreName = rig.NameWellbore,
                    WellboreUid = rig.UidWellbore,
                    WellName = rig.NameWell,
                    WellUid = rig.UidWell,
                    Uid = rig.Uid
                }).OrderBy(rig => rig.Name);
        }

        public async Task<Rig> GetRig(string wellUid, string wellboreUid, string rigUid)
        {
            var query = RigQueries.GetWitsmlRigById(wellUid, wellboreUid, rigUid);
            var result = await WitsmlClient.GetFromStoreAsync(query, new OptionsIn(ReturnElements.All));
            var witsmlRig = result.Rigs.FirstOrDefault();
            if (witsmlRig == null) return null;

            return new Rig
            {
                AirGap = witsmlRig.AirGap,
                DateTimeCreation = StringHelpers.ToDateTime(witsmlRig.CommonData.DTimCreation),
                DateTimeLastChange = StringHelpers.ToDateTime(witsmlRig.CommonData.DTimLastChange),
                ItemState = witsmlRig.CommonData.ItemState,
                Name = witsmlRig.Name,
                Owner = witsmlRig.Owner,
                TypeRig = witsmlRig.TypeRig,
                WellboreName = witsmlRig.NameWellbore,
                WellboreUid = witsmlRig.UidWellbore,
                WellName = witsmlRig.NameWell,
                WellUid = witsmlRig.UidWell,
                Uid = witsmlRig.Uid
            };
        }
    }
}
