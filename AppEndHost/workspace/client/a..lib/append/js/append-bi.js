// ============================================
// BaseInfo Management
// ============================================
// This module handles BaseInfo-related operations including:
// - Retrieving BaseInfo items by ID, Name, or Parent
// - Building BaseInfo queries and options

function getBiById(id) {
    if (fixNull(id,'') === '') return {};
    let options = getBiReadByKeyOptions(id);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'][0];
    } else {
        return {};
    }
}

function getBiItemsByParentId(parentId) {
    if (fixNull(parentId,'') === '') return {};
    let options = getBiReadListOptions("ParentId", parentId, 500);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'];
    } else {
        return {};
    }
}

function getBiByName(shortName) {
    if (fixNull(shortName,'') === '') return {};
    let options = getBiReadListOptions("ShortName", shortName, 1);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'][0];
    } else {
        return {};
    }
}

function getBiItemsByParentShortName(parentShortName) {
    if (fixNull(parentShortName,'') === '') return {};
    let parObj = getBiByName(parentShortName);
    if (fixNull(parent, '') !== '') {
        let parentId = parObj["Id"];
        return getBiItemsByParentId(parentId);
    } else {
        return {};
    }
}

function getBiReadListOptions(fieldName, fieldValue, pageSize) {
    let mName = getBiReadListMethod();
    let options = {};
    options.requests = [{
        Method: mName,
        cacheTime: shared.biCacheTime,
        Inputs: {
            ClientQueryJE: {
                QueryFullName: mName,
                Where: {
                    ConjunctiveOperator: "AND",
                    CompareClauses: [{ Name: fieldName, Value: fieldValue, CompareOperator: "Equal" }]
                },
                OrderClauses: [{ Name: "ViewOrder", OrderDirection: "ASC" }],
                Pagination: { PageNumber: 1, PageSize: pageSize },
                ExceptAggregations: ["Count"],
                IncludeSubQueries: false
            }
        }
    }];
    return options;
}

function getBiReadByKeyOptions(id) {
    let mName = getBiReadByKeyMethod();
    return {
        requests: [{
            Method: mName,
            cacheTime: shared.biCacheTime,
            Inputs: {
                ClientQueryJE: {
                    QueryFullName: mName,
                    Params: [{ Name: "Id", Value: id }]
                }
            }
        }]
    };
}

function getBiReadListMethod() {
    let m = `${shared.defaultDb}.${shared.biClass}.ReadList`;
    return m;
}

function getBiReadByKeyMethod() {
    let m = `${shared.defaultDb}.${shared.biClass}.ReadByKey`;
    return m;
}
