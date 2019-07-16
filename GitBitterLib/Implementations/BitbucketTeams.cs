namespace GitBitterLib
{
    using System.Collections.Generic;
    using SharpBucket.V2.Pocos;
    using SharpBucket.V2.EndPoints;
    using SharpBucket.V2;

    public class BitbucketTeamsEndPoint : EndPoint
    {
        public BitbucketTeamsEndPoint(SharpBucketV2 sharpBucketV2)
            : base(sharpBucketV2, "teams/")
        {
        }

        public List<Team> GetUserTeamsWithContributorRole(int max = 0)
        {
            var parameters = new Dictionary<string, object> { { "role", "contributor" } };
            return GetPaginatedValues<Team>(_baseUrl, max, parameters);
        }
    }
}