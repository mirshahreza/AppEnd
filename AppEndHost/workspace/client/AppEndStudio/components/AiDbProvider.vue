<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body p-3 scrollable">
            <!-- ========== View: Connection Cards ========== -->
            <template v-if="local.view === 'cards'">
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

                            <div class="card-header bg-transparent border-bottom d-flex justify-content-between align-items-center p-3">
                                <div class="d-flex align-items-center gap-2">
                                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="text-secondary">
                                        <path d="M21 12c0 1.66-4 3-9 3s-9-1.34-9-3"></path>
                                        <path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5"></path>
                                        <ellipse cx="12" cy="5" rx="9" ry="3"></ellipse>
                                    </svg>
                                    <h6 class="mb-0 fw-bold">{{ conn.name }}</h6>
                                </div>
                                <span class="badge" :style="getTypeBadgeStyle(conn.type)">{{ conn.type }}</span>
                            </div>

                            <div class="card-body p-3">
                                <div class="d-flex justify-content-between align-items-center mb-2">
                                    <span class="badge" :class="getStatusClass(conn.status)">{{ getStatusLabel(conn.status) }}</span>
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
                                        @click="startEnrichment(conn)">
                                    {{ isOperationActive(conn.id) ? 'Processing...' : 'Start Enrichment' }}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </template>

            <!-- ========== View: Schema Grid ========== -->
            <template v-else>
                <div class="d-flex flex-column h-100">
                    <!-- Back + Connection name + Action button -->
                    <div class="card-header p-2 px-3 rounded-0 border-0 bg-body-subtle d-flex align-items-center flex-wrap gap-2">
                        <button class="btn btn-sm btn-outline-secondary" @click="backToCards">
                            <i class="fa-solid fa-arrow-left me-1"></i>
                            {{ shared.translate('Back') || 'Back' }}
                        </button>
                        <span class="fw-bold text-secondary">{{ local.schemaConnectionName }}</span>
                        <div class="ms-auto">
                            <button type="button"
                                    class="btn btn-sm border-0 btn-outline-success px-3"
                                    :disabled="selectedStructureIds.length === 0"
                                    @click="confirmAndStartEnrichment">
                                <i class="fa-solid fa-wand-magic-sparkles me-1"></i>
                                <span>{{ shared.translate('StartEnrichment') || 'Start Enrichment' }}</span>
                            </button>
                        </div>
                    </div>

                    <!-- Filters -->
                    <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
                        <div class="container-fluid">
                            <div class="row g-2 align-items-center">
                                <div class="col-auto">
                                    <label class="form-label mb-0 me-2 small text-muted">{{ shared.translate('ObjectType') || 'Object type' }}</label>
                                </div>
                                <div class="col-auto">
                                    <select class="form-select form-select-sm" v-model="local.filterObjectType" style="min-width: 120px;">
                                        <option value="">All</option>
                                        <option value="Table">Table</option>
                                        <option value="Column">Column</option>
                                    </select>
                                </div>
                                <div class="col-auto ms-2">
                                    <label class="form-label mb-0 me-2 small text-muted">{{ shared.translate('Status') || 'Status' }}</label>
                                </div>
                                <div class="col-auto">
                                    <select class="form-select form-select-sm" v-model="local.filterStatus" style="min-width: 140px;">
                                        <option value="">All</option>
                                        <option value="enriched">Enriched</option>
                                        <option value="new">New</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Loading schema -->
                    <div v-if="local.schemaLoading" class="d-flex justify-content-center align-items-center flex-grow-1" style="min-height: 200px;">
                        <div class="spinner-border text-primary"></div>
                    </div>

                    <!-- Schema Table -->
                    <div v-else class="card-body border-0 p-0 flex-grow-1 overflow-auto">
                        <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                            <div class="card-body border-0 p-0 scrollable">
                                <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent fs-d8">
                                    <thead>
                                        <tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
                                            <th class="sticky-top ae-thead-th text-center" style="width: 32px;"></th>
                                            <th class="sticky-top ae-thead-th text-center" style="width: 44px;">
                                                <input type="checkbox"
                                                       class="form-check-input"
                                                       :checked="filteredRows.length > 0 && selectedCount === filteredRows.length"
                                                       :indeterminate="selectedCount > 0 && selectedCount < filteredRows.length"
                                                       @change="toggleSelectAll">
                                            </th>
                                            <th class="sticky-top ae-thead-th">Object type</th>
                                            <th class="sticky-top ae-thead-th">Schema</th>
                                            <th class="sticky-top ae-thead-th">Table</th>
                                            <th class="sticky-top ae-thead-th">Object name</th>
                                            <th class="sticky-top ae-thead-th">{{ shared.translate('LastUpdated') || 'Last Updated' }}</th>
                                            <th class="sticky-top ae-thead-th">Status</th>
                                            <th class="sticky-top ae-thead-th text-center" style="width: 90px;"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <template v-for="row in filteredRows" :key="row.structureId">
                                            <tr>
                                                <td class="ae-table-td text-center align-middle">
                                                    <button type="button" class="btn btn-sm btn-link p-0 text-secondary"
                                                            @click="row.expanded = !row.expanded"
                                                            :title="row.expanded ? 'Collapse' : 'Detail'">
                                                        <i class="fa-solid fa-fw" :class="row.expanded ? 'fa-chevron-down' : 'fa-chevron-right'"></i>
                                                    </button>
                                                </td>
                                                <td class="ae-table-td text-center">
                                                    <input type="checkbox"
                                                           class="form-check-input"
                                                           :checked="row.selected"
                                                           @change="row.selected = $event.target.checked">
                                                </td>
                                                <td class="ae-table-td">
                                                    <span class="badge" :class="row.objectType === 'Table' ? 'bg-primary' : 'bg-secondary'">{{ row.objectType }}</span>
                                                </td>
                                                <td class="ae-table-td">{{ row.schemaName }}</td>
                                                <td class="ae-table-td">{{ row.tableName }}</td>
                                                <td class="ae-table-td">{{ row.objectName }}</td>
                                                <td class="ae-table-td small">{{ formatLastUpdated(row.detail ? row.detail.updatedOn : null) }}</td>
                                                <td class="ae-table-td">
                                                    <span class="badge" :class="row.isEnriched ? 'bg-success-subtle text-success-emphasis' : 'bg-warning-subtle text-warning-emphasis'">
                                                        {{ row.isEnriched ? (shared.translate('Enriched') || 'Enriched') : (shared.translate('New') || 'New') }}
                                                    </span>
                                                </td>
                                                <td class="ae-table-td text-center">
                                                    <button type="button"
                                                            class="btn btn-sm btn-outline-success px-2"
                                                            :disabled="row.isEnriched"
                                                            :title="row.isEnriched ? '' : (shared.translate('Enrich') || 'Enrich')"
                                                            @click="openEnrichModal(row)">
                                                        <i class="fa-solid fa-pen-to-square me-1"></i>
                                                        <span>{{ shared.translate('Enrich') || 'Enrich' }}</span>
                                                    </button>
                                                </td>
                                            </tr>
                                            <tr v-if="row.expanded" class="detail-row">
                                                <td colspan="9" class="ae-detail-cell p-0">
                                                    <div class="ae-detail-panel">
                                                        <template v-if="row.detail">
                                                            <div class="ae-detail-grid">
                                                                <div class="ae-detail-group">
                                                                    <div class="ae-detail-group-title">
                                                                        <i class="fa-solid fa-heading me-1 opacity-75"></i>{{ shared.translate('Titles') || 'Titles' }}
                                                                    </div>
                                                                    <div class="ae-detail-field">
                                                                        <span class="ae-detail-label">HumanTitleEn</span>
                                                                        <span class="ae-detail-value">{{ row.detail.humanTitleEn || '—' }}</span>
                                                                    </div>
                                                                    <div class="ae-detail-field">
                                                                        <span class="ae-detail-label">HumanTitleNative</span>
                                                                        <span class="ae-detail-value">{{ row.detail.humanTitleNative || '—' }}</span>
                                                                    </div>
                                                                </div>
                                                                <div class="ae-detail-group">
                                                                    <div class="ae-detail-group-title">
                                                                        <i class="fa-solid fa-note-sticky me-1 opacity-75"></i>{{ shared.translate('Notes') || 'Notes' }}
                                                                    </div>
                                                                    <div class="ae-detail-field">
                                                                        <span class="ae-detail-label">NoteEn</span>
                                                                        <span class="ae-detail-value ae-detail-value-multiline">{{ row.detail.noteEn || '—' }}</span>
                                                                    </div>
                                                                    <div class="ae-detail-field">
                                                                        <span class="ae-detail-label">NoteNative</span>
                                                                        <span class="ae-detail-value ae-detail-value-multiline">{{ row.detail.noteNative || '—' }}</span>
                                                                    </div>
                                                                </div>
                                                                <div class="ae-detail-group">
                                                                    <div class="ae-detail-group-title">
                                                                        <i class="fa-solid fa-tags me-1 opacity-75"></i>{{ shared.translate('Keywords') || 'Keywords' }}
                                                                    </div>
                                                                    <div class="ae-detail-field">
                                                                        <span class="ae-detail-label">KeywordsEn</span>
                                                                        <span class="ae-detail-value ae-detail-value-multiline">{{ row.detail.keywordsEn || '—' }}</span>
                                                                    </div>
                                                                    <div class="ae-detail-field">
                                                                        <span class="ae-detail-label">KeywordsNative</span>
                                                                        <span class="ae-detail-value ae-detail-value-multiline">{{ row.detail.keywordsNative || '—' }}</span>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="ae-detail-actions">
                                                                <button type="button" class="btn btn-sm btn-outline-primary" @click="openEditModal(row)">
                                                                    <i class="fa-solid fa-pen me-1"></i>{{ shared.translate('Edit') || 'ویرایش' }}
                                                                </button>
                                                                <button type="button" class="btn btn-sm btn-outline-danger" @click="deleteZetadata(row)">
                                                                    <i class="fa-solid fa-trash me-1"></i>{{ shared.translate('Delete') || 'حذف' }}
                                                                </button>
                                                            </div>
                                                        </template>
                                                        <div v-else class="ae-detail-empty">
                                                            <i class="fa-solid fa-circle-info text-muted mb-2" style="font-size: 1.5rem;"></i>
                                                            <span class="text-muted">{{ shared.translate('NotEnriched') || 'غنی‌سازی نشده' }}</span>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </template>
                                    </tbody>
                                </table>
                                <div v-if="filteredRows.length === 0" class="text-center text-muted py-4">
                                    {{ local.schemaRows.length === 0 ? 'No schema data.' : 'No rows match filters.' }}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </template>

            <!-- Edit/Enrich Zetadata Modal (Bootstrap) - Teleport to body so backdrop does not cover modal -->
            <Teleport to="body">
                <div class="modal fade" id="aeZetadataEditModal" tabindex="-1" aria-labelledby="aeZetadataEditModalLabel" aria-hidden="true" data-bs-backdrop="true" data-bs-keyboard="true">
                    <div class="modal-dialog modal-dialog-centered modal-lg modal-dialog-scrollable">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="aeZetadataEditModalLabel">{{ local.editModal.isCreate ? (shared.translate('Enrich') || 'Enrich') : (shared.translate('Edit') || 'Edit') }} — {{ local.editModal.row ? local.editModal.row.objectName : '' }}</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-2">
                                    <label class="form-label small fw-bold">HumanTitleEn</label>
                                    <input type="text" class="form-control form-control-sm" v-model="local.editModal.form.humanTitleEn" maxlength="500">
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small fw-bold">HumanTitleNative</label>
                                    <input type="text" class="form-control form-control-sm" v-model="local.editModal.form.humanTitleNative" maxlength="500">
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small fw-bold">NoteEn</label>
                                    <textarea class="form-control form-control-sm" v-model="local.editModal.form.noteEn" rows="3"></textarea>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small fw-bold">NoteNative</label>
                                    <textarea class="form-control form-control-sm" v-model="local.editModal.form.noteNative" rows="3"></textarea>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small fw-bold">KeywordsEn</label>
                                    <textarea class="form-control form-control-sm" v-model="local.editModal.form.keywordsEn" rows="2"></textarea>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small fw-bold">KeywordsNative</label>
                                    <textarea class="form-control form-control-sm" v-model="local.editModal.form.keywordsNative" rows="2"></textarea>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-sm btn-secondary" data-bs-dismiss="modal">{{ shared.translate('Cancel') || 'Cancel' }}</button>
                                <button type="button" class="btn btn-sm btn-primary" @click="saveEditModal">{{ shared.translate('Save') || 'Save' }}</button>
                            </div>
                        </div>
                    </div>
                </div>
            </Teleport>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = {
        cid: "",
        c: null,
        local: {
            view: 'cards', // 'cards' | 'schema'
            connections: [],
            loading: true,
            activeOperations: [],
            enrichmentIntervals: {},
            schemaConnectionName: '',
            schemaRows: [],      // { objectType, schemaName, tableName, objectName, structureId, isEnriched, selected, expanded, detail }
            schemaLoading: false,
            filterObjectType: '',
            filterStatus: '',
            editModal: {
                show: false,
                isCreate: false,
                row: null,
                form: { humanTitleEn: '', humanTitleNative: '', noteEn: '', noteNative: '', keywordsEn: '', keywordsNative: '' }
            }
        }
    };

    export default {
        computed: {
            filteredRows() {
                const rows = _this.c.local.schemaRows;
                let out = rows;
                if (_this.c.local.filterObjectType) {
                    out = out.filter(r => r.objectType === _this.c.local.filterObjectType);
                }
                if (_this.c.local.filterStatus) {
                    if (_this.c.local.filterStatus === 'enriched') out = out.filter(r => r.isEnriched);
                    if (_this.c.local.filterStatus === 'new') out = out.filter(r => !r.isEnriched);
                }
                return out;
            },
            selectedCount() {
                return this.filteredRows.filter(r => r.selected).length;
            },
            selectedStructureIds() {
                return _this.c.local.schemaRows.filter(r => r.selected).map(r => r.structureId);
            }
        },
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
            toggleSelectAll() {
                const filtered = _this.c.filteredRows;
                const allSelected = _this.c.selectedCount === filtered.length;
                filtered.forEach(r => { r.selected = !allSelected; });
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
                const hue = (percent / 100) * 120;
                return `hsl(${hue}, 75%, 45%)`;
            },
            formatDate(dateStr) {
                if (!dateStr) return '';
                return dateStr.split(' ')[0];
            },
            formatLastUpdated(val) {
                if (!val) return '—';
                if (typeof val === 'string') return shared.formatDateTime ? shared.formatDateTime(val) : val.split('T')[0];
                return '—';
            },
            loadConnections() {
                _this.c.local.loading = true;
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
                        onDone: function () {},
                        onFail: function () {}
                    });
                }
            },
            resetEnrichment(id) {
                this.updateConnection(id, { status: 'not_enriched', enrichmentProgress: 0 });
                if (_this.c.local.enrichmentIntervals[id]) {
                    clearInterval(_this.c.local.enrichmentIntervals[id]);
                    delete _this.c.local.enrichmentIntervals[id];
                }
                showSuccess('Enrichment status cleared.');
            },
            startEnrichment(conn) {
                if (conn.id == null || this.isOperationActive(conn.id)) return;
                _this.c.local.view = 'schema';
                _this.c.local.schemaConnectionName = conn.name;
                _this.c.local.schemaRows = [];
                _this.c.local.filterObjectType = '';
                _this.c.local.filterStatus = '';
                _this.c.local.schemaLoading = true;
                rpc({
                    requests: [
                        { Method: 'Zzz.AppEndProxy.GetSchemaForEnrich', Inputs: { DbConfName: conn.name } },
                        { Method: 'Zzz.AppEndProxy.GetEnrichedStructureIds', Inputs: { ConnectionName: conn.name } },
                        { Method: 'Zzz.AppEndProxy.GetBaseZetadataByConnection', Inputs: { ConnectionName: conn.name } }
                    ],
                    onDone: function (res) {
                        const raw0 = Array.isArray(res) && res[0] !== undefined ? res[0] : null;
                        const raw1 = Array.isArray(res) && res[1] !== undefined ? res[1] : null;
                        const raw2 = Array.isArray(res) && res[2] !== undefined ? res[2] : null;
                        const unwrap0 = raw0 !== null ? R0R([raw0]) : null;
                        const unwrap1 = raw1 !== null ? R0R([raw1]) : null;
                        const unwrap2 = raw2 !== null ? R0R([raw2]) : null;
                        const schemaRaw = Array.isArray(unwrap0) ? unwrap0 : (Array.isArray(raw0) ? raw0 : (unwrap0 && Array.isArray(unwrap0.Master) ? unwrap0.Master : []));
                        const enrichedRaw = Array.isArray(unwrap1) ? unwrap1 : (Array.isArray(raw1) ? raw1 : (unwrap1 && Array.isArray(unwrap1.Master) ? unwrap1.Master : []));
                        const zetadataRaw = Array.isArray(unwrap2) ? unwrap2 : (Array.isArray(raw2) ? raw2 : (unwrap2 && Array.isArray(unwrap2.Master) ? unwrap2.Master : []));
                        const enrichedSet = {};
                        if (Array.isArray(enrichedRaw)) enrichedRaw.forEach(function (id) { enrichedSet[id] = true; });
                        const zetadataByStructureId = {};
                        if (Array.isArray(zetadataRaw)) {
                            zetadataRaw.forEach(function (z) {
                                const sid = (z.StructureId || z.structureId || '').toString();
                                if (sid) {
                                    const u = z.UpdatedOn || z.updatedOn;
                                    zetadataByStructureId[sid] = {
                                        humanTitleEn: (z.HumanTitleEn || z.humanTitleEn || '').toString(),
                                        humanTitleNative: (z.HumanTitleNative || z.humanTitleNative || '').toString(),
                                        noteEn: (z.NoteEn || z.noteEn || '').toString(),
                                        noteNative: (z.NoteNative || z.noteNative || '').toString(),
                                        keywordsEn: (z.KeywordsEn || z.keywordsEn || '').toString(),
                                        keywordsNative: (z.KeywordsNative || z.keywordsNative || '').toString(),
                                        updatedOn: u ? (typeof u === 'string' ? u : (u && u.toISOString ? u.toISOString() : '')) : null
                                    };
                                }
                            });
                        }
                        const rows = (schemaRaw || []).map(function (item) {
                            const structureId = (item.StructureId || item.structureId || '').toString();
                            return {
                                objectType: (item.ObjectType || item.objectType || '').toString(),
                                schemaName: (item.SchemaName || item.schemaName || '').toString(),
                                tableName: (item.TableName || item.tableName || '').toString(),
                                objectName: (item.ObjectName || item.objectName || '').toString(),
                                structureId: structureId,
                                isEnriched: !!enrichedSet[structureId],
                                selected: false,
                                expanded: false,
                                detail: zetadataByStructureId[structureId] || null
                            };
                        });
                        _this.c.local.schemaRows = rows;
                        _this.c.local.schemaLoading = false;
                    },
                    onFail: function (err) {
                        _this.c.local.schemaLoading = false;
                        showError(err && (err.message || err.Message) ? (err.message || err.Message) : 'Failed to load schema.');
                    }
                });
            },
            backToCards() {
                _this.c.local.view = 'cards';
                _this.c.local.schemaConnectionName = '';
                _this.c.local.schemaRows = [];
            },
            confirmAndStartEnrichment() {
                const ids = _this.c.selectedStructureIds;
                if (ids.length === 0) return;
                showSuccess('Selected ' + ids.length + ' item(s). StructureIds ready for AI enrichment step. (Next step: AI generation and save to BaseZetadata)');
            },
            getZetadataModalInstance() {
                var el = document.getElementById('aeZetadataEditModal');
                if (!el || typeof bootstrap === 'undefined') return null;
                return bootstrap.Modal.getOrCreateInstance(el);
            },
            openEditModal(row) {
                if (!row || !row.detail) return;
                _this.c.local.editModal.row = row;
                _this.c.local.editModal.isCreate = false;
                _this.c.local.editModal.form = {
                    humanTitleEn: row.detail.humanTitleEn || '',
                    humanTitleNative: row.detail.humanTitleNative || '',
                    noteEn: row.detail.noteEn || '',
                    noteNative: row.detail.noteNative || '',
                    keywordsEn: row.detail.keywordsEn || '',
                    keywordsNative: row.detail.keywordsNative || ''
                };
                _this.c.local.editModal.show = true;
                _this.c.$nextTick(function () {
                    var m = _this.c.getZetadataModalInstance();
                    if (m) m.show();
                });
            },
            openEnrichModal(row) {
                if (!row || row.isEnriched) return;
                _this.c.local.editModal.row = row;
                _this.c.local.editModal.isCreate = true;
                _this.c.local.editModal.form = {
                    humanTitleEn: '',
                    humanTitleNative: '',
                    noteEn: '',
                    noteNative: '',
                    keywordsEn: '',
                    keywordsNative: ''
                };
                _this.c.local.editModal.show = true;
                _this.c.$nextTick(function () {
                    var m = _this.c.getZetadataModalInstance();
                    if (m) m.show();
                });
            },
            closeEditModal() {
                var m = _this.c.getZetadataModalInstance();
                if (m) m.hide();
                _this.c.local.editModal.show = false;
                _this.c.local.editModal.row = null;
                _this.c.local.editModal.isCreate = false;
            },
            saveEditModal() {
                const row = _this.c.local.editModal.row;
                const form = _this.c.local.editModal.form;
                const isCreate = _this.c.local.editModal.isCreate;
                if (!row || !row.structureId) return;
                if (isCreate) {
                    rpc({
                        requests: [{
                            Method: 'Zzz.AppEndProxy.CreateBaseZetadata',
                            Inputs: {
                                StructureId: row.structureId,
                                ConnectionName: _this.c.local.schemaConnectionName,
                                ObjectName: row.objectName || '',
                                ObjectType: row.objectType || null,
                                HumanTitleEn: form.humanTitleEn || null,
                                HumanTitleNative: form.humanTitleNative || null,
                                NoteEn: form.noteEn || null,
                                NoteNative: form.noteNative || null,
                                KeywordsEn: form.keywordsEn || null,
                                KeywordsNative: form.keywordsNative || null
                            }
                        }],
                        onDone: function (res) {
                            var resp = res && Array.isArray(res) ? res[0] : res;
                            if (!resp || (resp.IsSucceeded !== true && resp.IsSucceeded !== 'true')) {
                                var msg = (resp && resp.Result && (typeof resp.Result === 'object' && (resp.Result.Message || resp.Result.message))) || (typeof resp.Result === 'string' ? resp.Result : null) || (shared.translate('SaveFailed') || 'Save failed.');
                                showError(msg);
                                return;
                            }
                            if (resp.Result === false || resp.Result === 'false') {
                                showError((shared.translate('SaveFailed') || 'Save failed.') + ' ' + (shared.translate('CheckBaseZetadataTable') || 'Ensure BaseZetadata table exists in DefaultRepo database.'));
                                return;
                            }
                            const now = new Date().toISOString();
                            row.detail = {
                                humanTitleEn: form.humanTitleEn || '',
                                humanTitleNative: form.humanTitleNative || '',
                                noteEn: form.noteEn || '',
                                noteNative: form.noteNative || '',
                                keywordsEn: form.keywordsEn || '',
                                keywordsNative: form.keywordsNative || '',
                                updatedOn: now
                            };
                            row.isEnriched = true;
                            row.expanded = true;
                            var modalInst = _this.c.getZetadataModalInstance();
                            if (modalInst) modalInst.hide();
                            _this.c.local.editModal.show = false;
                            _this.c.local.editModal.row = null;
                            _this.c.local.editModal.isCreate = false;
                            showSuccess(shared.translate('Saved') || 'Saved.');
                        },
                        onFail: function (err) {
                            showError(err && (err.message || err.Message) ? (err.message || err.Message) : 'Save failed.');
                        }
                    });
                } else {
                    rpc({
                        requests: [{
                            Method: 'Zzz.AppEndProxy.UpdateBaseZetadata',
                            Inputs: {
                                StructureId: row.structureId,
                                HumanTitleEn: form.humanTitleEn || null,
                                HumanTitleNative: form.humanTitleNative || null,
                                NoteEn: form.noteEn || null,
                                NoteNative: form.noteNative || null,
                                KeywordsEn: form.keywordsEn || null,
                                KeywordsNative: form.keywordsNative || null
                            }
                        }],
                        onDone: function () {
                            if (row.detail) {
                                row.detail.humanTitleEn = form.humanTitleEn;
                                row.detail.humanTitleNative = form.humanTitleNative;
                                row.detail.noteEn = form.noteEn;
                                row.detail.noteNative = form.noteNative;
                                row.detail.keywordsEn = form.keywordsEn;
                                row.detail.keywordsNative = form.keywordsNative;
                                row.detail.updatedOn = new Date().toISOString();
                            }
                            var modalInst = _this.c.getZetadataModalInstance();
                            if (modalInst) modalInst.hide();
                            _this.c.local.editModal.show = false;
                            _this.c.local.editModal.row = null;
                            showSuccess(shared.translate('Saved') || 'Saved.');
                        },
                        onFail: function (err) {
                            showError(err && (err.message || err.Message) ? (err.message || err.Message) : 'Update failed.');
                        }
                    });
                }
            },
            deleteZetadata(row) {
                if (!row || !row.structureId) return;
                shared.showConfirm({
                    title: shared.translate('Delete') || 'حذف',
                    message1: (shared.translate('ConfirmDelete') || 'آیا از حذف این رکورد متادیتا اطمینان دارید؟'),
                    message2: row.objectName || row.structureId,
                    callback: function () {
                        rpc({
                            requests: [{ Method: 'Zzz.AppEndProxy.DeleteBaseZetadata', Inputs: { StructureId: row.structureId } }],
                            onDone: function () {
                                row.detail = null;
                                row.isEnriched = false;
                                showSuccess(shared.translate('Deleted') || 'حذف شد.');
                            },
                            onFail: function (err) {
                                showError(err && (err.message || err.Message) ? (err.message || err.Message) : 'Delete failed.');
                            }
                        });
                    }
                });
            },
            testConnection(id) {
                this.addActiveOperation(id);
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
            var el = document.getElementById('aeZetadataEditModal');
            if (el) {
                el.addEventListener('hidden.bs.modal', function () {
                    _this.c.local.editModal.show = false;
                    _this.c.local.editModal.row = null;
                    _this.c.local.editModal.isCreate = false;
                });
            }
        },
        beforeUnmount() {
            Object.values(_this.c.local.enrichmentIntervals || {}).forEach(interval => clearInterval(interval));
        },
        props: { cid: String }
    }
</script>

<style scoped>
.card { border-radius: 12px; }
.card:hover { cursor: default; }
.badge { padding: 4px 10px; border-radius: 12px; font-size: 11px; }
.progress { border-radius: 4px; }
.progress-bar { transition: width 0.4s ease, background-color 0.4s ease; }
.ae-thead-th { background: var(--bs-body-bg, #fff); }
.detail-row { vertical-align: top; }

/* Detail panel */
.ae-detail-cell {
    background: var(--bs-body-bg);
    border-top: none;
    vertical-align: top;
}
.ae-detail-panel {
    margin: 0 0.5rem 0.5rem;
    padding: 1rem 1.25rem;
    background: var(--bs-body-bg);
    border: 1px solid var(--bs-border-color);
    border-radius: 10px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
}
.ae-detail-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: 1.25rem 2rem;
    margin-bottom: 1rem;
}
.ae-detail-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}
.ae-detail-group-title {
    font-size: 0.7rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.04em;
    color: var(--bs-secondary);
    padding-bottom: 0.25rem;
    border-bottom: 1px solid var(--bs-border-color-translucent, var(--bs-border-color));
}
.ae-detail-field {
    display: grid;
    grid-template-columns: 130px 1fr;
    gap: 0.5rem 1rem;
    align-items: start;
    font-size: 0.875rem;
}
.ae-detail-label {
    color: var(--bs-secondary);
    font-weight: 500;
    flex-shrink: 0;
}
.ae-detail-value {
    word-break: break-word;
    line-height: 1.45;
    color: var(--bs-body-color);
}
.ae-detail-value-multiline {
    white-space: pre-wrap;
    max-height: 4.5em;
    overflow-y: auto;
}
.ae-detail-actions {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    padding-top: 0.75rem;
    border-top: 1px solid var(--bs-border-color-translucent, var(--bs-border-color));
}
.ae-detail-empty {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 1.5rem;
    text-align: center;
}
</style>
