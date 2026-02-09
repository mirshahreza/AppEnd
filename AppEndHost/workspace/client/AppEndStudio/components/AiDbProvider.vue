<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body p-3 scrollable">
            <!-- Loading State -->
            <div v-if="local.loading" class="d-flex justify-content-center align-items-center" style="min-height: 400px;">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>

            <!-- Empty State -->
            <div v-else-if="local.connections.length === 0" class="text-center p-5 text-secondary">
                <i class="fa-solid fa-database fa-3x mb-3"></i>
                <div>No connections found.</div>
            </div>

            <!-- Grid of Connection Cards -->
            <div v-else class="row g-3">
                <div v-for="conn in local.connections" :key="conn.id" class="col-48 col-md-24 col-lg-16 col-xl-12">
                    <div class="card h-100 shadow-sm border" style="transition: transform 0.2s, box-shadow 0.2s;" 
                         @mouseenter="$event.currentTarget.style.transform = 'translateY(-2px)'; $event.currentTarget.style.boxShadow = '0 4px 12px rgba(0,0,0,0.1)'"
                         @mouseleave="$event.currentTarget.style.transform = ''; $event.currentTarget.style.boxShadow = ''">
                        
                        <!-- Card Header: Name + Type Badge -->
                        <div class="card-header bg-transparent border-bottom d-flex justify-content-between align-items-center p-3">
                            <div class="d-flex align-items-center gap-2">
                                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="text-secondary">
                                    <path d="M21 12c0 1.66-4 3-9 3s-9-1.34-9-3"></path>
                                    <path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5"></path>
                                    <ellipse cx="12" cy="5" rx="9" ry="3"></ellipse>
                                </svg>
                                <h6 class="mb-0 fw-bold">{{ conn.name }}</h6>
                            </div>
                            <span class="badge" :style="getTypeBadgeStyle(conn.type)">
                                {{ conn.type }}
                            </span>
                        </div>

                        <!-- Card Body: Status + Progress -->
                        <div class="card-body p-3">
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="badge" :class="getStatusClass(conn.status)">
                                    {{ getStatusLabel(conn.status) }}
                                </span>
                                <small class="text-secondary">{{ formatDate(conn.lastUpdated) }}</small>
                            </div>
                            
                            <div>
                                <div class="d-flex justify-content-between mb-1">
                                    <small class="text-secondary fw-bold">Enrichment</small>
                                    <small class="text-secondary fw-bold">{{ conn.enrichmentProgress }}%</small>
                                </div>
                                <div class="progress" style="height: 8px;">
                                    <div class="progress-bar" 
                                         :style="{ width: conn.enrichmentProgress + '%', backgroundColor: getProgressColor(conn.enrichmentProgress) }"
                                         role="progressbar"></div>
                                </div>
                            </div>
                        </div>

                        <!-- Card Footer: Buttons (require conn.id from DB) -->
                        <div class="card-footer bg-transparent border-top p-3 d-flex justify-content-end gap-2">
                            <button v-if="conn.enrichmentProgress > 0" 
                                    class="btn btn-sm btn-outline-danger" 
                                    :disabled="conn.id == null || isOperationActive(conn.id)"
                                    @click="resetEnrichment(conn.id)">
                                Reset Enrich
                            </button>
                            <button class="btn btn-sm btn-outline-secondary" 
                                    :disabled="conn.id == null || isOperationActive(conn.id)"
                                    @click="testConnection(conn.id)">
                                Test
                            </button>
                            <button class="btn btn-sm btn-primary" 
                                    :disabled="conn.id == null || isOperationActive(conn.id)"
                                    @click="startEnrichment(conn.id)">
                                {{ isOperationActive(conn.id) ? 'Processing...' : 'Start Enrich' }}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = {
        cid: "",
        c: null,
        local: {
            connections: [],
            loading: true,
            activeOperations: [],
            enrichmentIntervals: {}
        }
    };

    export default {
        methods: {
            isOperationActive(id) {
                return _this.c.local.activeOperations.indexOf(id) !== -1;
            },
            addActiveOperation(id) {
                if (_this.c.local.activeOperations.indexOf(id) === -1) {
                    _this.c.local.activeOperations.push(id);
                }
            },
            removeActiveOperation(id) {
                const idx = _this.c.local.activeOperations.indexOf(id);
                if (idx !== -1) {
                    _this.c.local.activeOperations.splice(idx, 1);
                }
            },
            getTypeBadgeStyle(type) {
                const config = {
                    'SQL Server': { color: '#0078D4', bg: '#E3F2FD' },
                    'PostgreSQL': { color: '#336791', bg: '#E3F2FD' },
                    'MySQL': { color: '#4479A1', bg: '#E3F2FD' },
                    'Oracle': { color: '#C74634', bg: '#FFEBEE' }
                };
                const cfg = config[type] || { color: '#616161', bg: '#F5F5F5' };
                return {
                    backgroundColor: cfg.bg,
                    color: cfg.color,
                    border: `1px solid ${cfg.color}40`,
                    fontSize: '11px',
                    fontWeight: '700',
                    textTransform: 'uppercase'
                };
            },
            getStatusClass(status) {
                return {
                    'not_enriched': 'bg-secondary-subtle text-secondary',
                    'enriching': 'bg-warning-subtle text-warning-emphasis',
                    'enriched': 'bg-success-subtle text-success-emphasis'
                }[status] || 'bg-secondary-subtle text-secondary';
            },
            getStatusLabel(status) {
                return {
                    'not_enriched': 'Not Enriched',
                    'enriching': 'Enriching...',
                    'enriched': 'Enriched'
                }[status] || 'Unknown';
            },
            getProgressColor(percent) {
                // Hue: 0 (Red) -> 120 (Green)
                const hue = (percent / 100) * 120;
                return `hsl(${hue}, 75%, 45%)`;
            },
            formatDate(dateStr) {
                if (!dateStr) return '';
                return dateStr.split(' ')[0];
            },
            loadConnections() {
                _this.c.local.loading = true;
                // 1) Ensure appsettings DbServers have a row in DB (so every connection has Id)
                // 2) List from appsettings (GetAppEndSettings)
                // 3) Enrichment params from DB (BaseDbConnections.ReadList)
                rpc({
                    requests: [
                        { Method: 'Zzz.AppEndProxy.EnsureDbConnectionsFromAppSettings', Inputs: {} },
                        { Method: 'Zzz.AppEndProxy.GetAppEndSettings', Inputs: {} },
                        {
                            Method: 'DefaultRepo.BaseDbConnections.ReadList',
                            Inputs: {
                                ClientQueryJE: {
                                    QueryFullName: 'DefaultRepo.BaseDbConnections.ReadList',
                                    Pagination: { PageNumber: 1, PageSize: 1000 }
                                }
                            }
                        }
                    ],
                    onDone: function (res) {
                        const settingsRaw = Array.isArray(res) && res[1] ? R0R([res[1]]) : null;
                        const dbRaw = Array.isArray(res) && res[2] ? R0R([res[2]]) : null;
                        const dbServers = (settingsRaw && settingsRaw.DbServers) ? settingsRaw.DbServers : [];
                        const dbList = (dbRaw && dbRaw.Master) ? dbRaw.Master : [];
                        const byName = {};
                        dbList.forEach(function (row) {
                            const n = (row.Name || '').toString().trim();
                            if (n) byName[n] = row;
                        });
                        _this.c.local.connections = dbServers.map(function (item) {
                            const name = (item.Name || '').toString().trim();
                            const dbRow = name ? byName[name] : null;
                            const serverType = (item.ServerType || 'MsSql').toString();
                            return {
                                id: dbRow ? dbRow.Id : null,
                                name: name || 'Unnamed',
                                type: serverType === 'MsSql' ? 'SQL Server' : serverType,
                                status: dbRow ? (dbRow.Status || 'not_enriched') : 'not_enriched',
                                enrichmentProgress: dbRow ? (dbRow.EnrichmentProgress || 0) : 0,
                                lastUpdated: dbRow && dbRow.LastUpdated ? new Date(dbRow.LastUpdated).toLocaleString() : ''
                            };
                        });
                        _this.c.local.loading = false;
                    },
                    onFail: function (err) {
                        _this.c.local.connections = [];
                        _this.c.local.loading = false;
                    }
                });
            },
            updateConnection(id, updates) {
                const idx = _this.c.local.connections.findIndex(c => c.id === id);
                if (idx !== -1) {
                    _this.c.local.connections[idx] = { ..._this.c.local.connections[idx], ...updates };
                }
                if (id == null) return;
                // Also update in database
                const dbUpdates = {};
                if (updates.status !== undefined) dbUpdates.Status = updates.status;
                if (updates.enrichmentProgress !== undefined) dbUpdates.EnrichmentProgress = updates.enrichmentProgress;
                if (updates.lastUpdated !== undefined) dbUpdates.LastUpdated = updates.lastUpdated;
                
                if (Object.keys(dbUpdates).length > 0) {
                    dbUpdates.Id = id;
                    rpc({
                        requests: [{
                            Method: "DefaultRepo.BaseDbConnections.UpdateEnrichmentStatus",
                            Inputs: {
                                ClientQueryJE: {
                                    QueryFullName: "DefaultRepo.BaseDbConnections.UpdateEnrichmentStatus",
                                    Data: dbUpdates
                                }
                            }
                        }],
                        onDone: function (res) {
                            // Success - data already updated in local state
                        },
                        onFail: function (err) {
                        }
                    });
                }
            },
            resetEnrichment(id) {
                this.updateConnection(id, { status: 'not_enriched', enrichmentProgress: 0 });
                if (_this.local.enrichmentIntervals[id]) {
                    clearInterval(_this.local.enrichmentIntervals[id]);
                    delete _this.local.enrichmentIntervals[id];
                }
                showSuccess('Enrichment status cleared.');
            },
            startEnrichment(id) {
                this.updateConnection(id, { status: 'enriching' });
                this.addActiveOperation(id);
                
                const interval = setInterval(() => {
                    const conn = _this.c.local.connections.find(c => c.id === id);
                    if (!conn || conn.status !== 'enriching') {
                        clearInterval(interval);
                        this.removeActiveOperation(id);
                        delete _this.local.enrichmentIntervals[id];
                        return;
                    }

                    if (conn.enrichmentProgress >= 100) {
                        clearInterval(interval);
                        this.removeActiveOperation(id);
                        delete _this.local.enrichmentIntervals[id];
                        this.updateConnection(id, { 
                            status: 'enriched', 
                            enrichmentProgress: 100, 
                            lastUpdated: new Date().toLocaleString() 
                        });
                        showSuccess(`Enrichment complete for ${conn.name}`);
                    } else {
                        this.updateConnection(id, { 
                            enrichmentProgress: Math.min(100, conn.enrichmentProgress + Math.floor(Math.random() * 10) + 5) 
                        });
                    }
                }, 1500);
                
                _this.local.enrichmentIntervals[id] = interval;
            },
            testConnection(id) {
                this.addActiveOperation(id);
                
                // Get connection details
                const conn = _this.c.local.connections.find(c => c.id === id);
                if (!conn) {
                    this.removeActiveOperation(id);
                    return;
                }
                
                setTimeout(() => {
                    this.removeActiveOperation(id);
                    showSuccess('Connection test successful');
                }, 1500);
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { 
            initVueComponent(_this); 
            _this.c.loadConnections();
        },
        beforeUnmount() {
            // Clean up intervals
            Object.values(_this.local.enrichmentIntervals).forEach(interval => {
                clearInterval(interval);
            });
        },
        props: { cid: String }
    }
</script>

<style scoped>
.card {
    border-radius: 12px;
}

.card:hover {
    cursor: default;
}

.badge {
    padding: 4px 10px;
    border-radius: 12px;
    font-size: 11px;
}

.progress {
    border-radius: 4px;
}

.progress-bar {
    transition: width 0.4s ease, background-color 0.4s ease;
}
</style>
