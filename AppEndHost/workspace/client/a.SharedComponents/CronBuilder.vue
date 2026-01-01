<template>
    <div :id="cid" class="cron-builder">
        <div class="card h-100 border-0 shadow-sm" style="border-radius: 4px;">
            <div class="card-body p-2">
                <!-- Schedule Type Tabs -->
                <div class="mb-2">
                    <div class="d-flex justify-content-center">
                        <div class="btn-group btn-group-sm" role="group">
                            <button type="button" 
                                    v-for="type in scheduleTypes" 
                                    :key="type.value"
                                    class="btn btn-sm"
                                    :class="scheduleType === type.value ? 'btn-primary' : 'btn-outline-secondary'"
                                    @click="changeScheduleType(type.value)"
                                    style="min-width: 65px; font-size: 0.8rem; padding: 0.25rem 0.5rem;">
                                {{ type.label }}
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Minutely Configuration -->
                <div v-if="scheduleType === 'minutely'">
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Run every N minutes</h6>
                        <div class="d-flex gap-2 align-items-center">
                            <label class="form-label mb-0" style="font-size: 0.85rem;">Every</label>
                            <input type="number" 
                                   class="form-control form-control-sm" 
                                   v-model.number="minuteInterval" 
                                   min="1" 
                                   max="59"
                                   style="width: 80px;">
                            <label class="form-label mb-0" style="font-size: 0.85rem;">minute(s)</label>
                        </div>
                        <small class="text-muted d-block mt-1" style="font-size: 0.75rem;">
                            Common intervals: 1, 5, 10, 15, 30 minutes
                        </small>
                        <div class="d-flex gap-1 mt-2 flex-wrap">
                            <button type="button" 
                                    v-for="interval in commonIntervals" 
                                    :key="interval"
                                    class="btn btn-sm"
                                    :class="minuteInterval === interval ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.5rem;"
                                    @click="minuteInterval = interval">
                                {{ interval }} min
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Hourly Configuration -->
                <div v-if="scheduleType === 'hourly'">
                    <!-- Minutes -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">
                            Minutes 
                            <span class="text-muted" style="font-size: 0.75rem;">(Common)</span>
                            <button type="button" class="btn btn-sm btn-link text-primary p-0 ms-1" @click="showAllMinutes = !showAllMinutes" style="font-size: 0.7rem;">
                                <i class="fa-solid fa-xs" :class="showAllMinutes ? 'fa-minus' : 'fa-plus'"></i>
                            </button>
                        </h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="minute in (showAllMinutes ? allMinutes : commonMinutes)" 
                                    :key="minute"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedMinutes.includes(minute) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleMinute(minute)">
                                {{ minute }}
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Weekly Configuration -->
                <div v-if="scheduleType === 'weekly'">
                    <!-- Days of Week -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Days of Week</h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="day in daysOfWeek" 
                                    :key="day.value"
                                    class="btn btn-sm px-2"
                                    :class="selectedDays.includes(day.value) ? 'btn-primary' : (day.isWeekend ? 'btn-outline-secondary text-danger' : 'btn-outline-secondary')"
                                    style="border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.5rem;"
                                    @click="toggleDay(day.value)">
                                {{ day.label }}
                            </button>
                        </div>
                    </div>

                    <!-- Hours -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Hours</h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="hour in hours" 
                                    :key="hour"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedHours.includes(hour) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleHour(hour)">
                                {{ hour }}
                            </button>
                        </div>
                    </div>

                    <!-- Minutes -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">
                            Minutes 
                            <span class="text-muted" style="font-size: 0.75rem;">(Common)</span>
                            <button type="button" class="btn btn-sm btn-link text-primary p-0 ms-1" @click="showAllMinutes = !showAllMinutes" style="font-size: 0.7rem;">
                                <i class="fa-solid fa-xs" :class="showAllMinutes ? 'fa-minus' : 'fa-plus'"></i>
                            </button>
                        </h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="minute in (showAllMinutes ? allMinutes : commonMinutes)" 
                                    :key="minute"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedMinutes.includes(minute) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleMinute(minute)">
                                {{ minute }}
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Daily Configuration -->
                <div v-if="scheduleType === 'daily'">
                    <!-- Hours -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Hours</h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="hour in hours" 
                                    :key="hour"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedHours.includes(hour) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleHour(hour)">
                                {{ hour }}
                            </button>
                        </div>
                    </div>

                    <!-- Minutes -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">
                            Minutes 
                            <span class="text-muted" style="font-size: 0.75rem;">(Common)</span>
                            <button type="button" class="btn btn-sm btn-link text-primary p-0 ms-1" @click="showAllMinutes = !showAllMinutes" style="font-size: 0.7rem;">
                                <i class="fa-solid fa-xs" :class="showAllMinutes ? 'fa-minus' : 'fa-plus'"></i>
                            </button>
                        </h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="minute in (showAllMinutes ? allMinutes : commonMinutes)" 
                                    :key="minute"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedMinutes.includes(minute) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleMinute(minute)">
                                {{ minute }}
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Monthly Configuration -->
                <div v-if="scheduleType === 'monthly'">
                    <!-- Days of Month -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Days of Month</h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="day in daysOfMonth" 
                                    :key="day"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedDaysOfMonth.includes(day) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 38px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.3rem;"
                                    @click="toggleDayOfMonth(day)">
                                {{ day }}
                            </button>
                        </div>
                    </div>

                    <!-- Hours -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Hours</h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="hour in hours" 
                                    :key="hour"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedHours.includes(hour) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleHour(hour)">
                                {{ hour }}
                            </button>
                        </div>
                    </div>

                    <!-- Minutes -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">
                            Minutes 
                            <span class="text-muted" style="font-size: 0.75rem;">(Common)</span>
                            <button type="button" class="btn btn-sm btn-link text-primary p-0 ms-1" @click="showAllMinutes = !showAllMinutes" style="font-size: 0.7rem;">
                                <i class="fa-solid fa-xs" :class="showAllMinutes ? 'fa-minus' : 'fa-plus'"></i>
                            </button>
                        </h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="minute in (showAllMinutes ? allMinutes : commonMinutes)" 
                                    :key="minute"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedMinutes.includes(minute) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleMinute(minute)">
                                {{ minute }}
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Yearly Configuration -->
                <div v-if="scheduleType === 'yearly'">
                    <!-- Months -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Months</h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="month in months" 
                                    :key="month.value"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedMonths.includes(month.value) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.4rem;"
                                    @click="toggleMonth(month.value)">
                                {{ month.label }}
                            </button>
                        </div>
                    </div>

                    <!-- Days of Month -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Days of Month</h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="day in daysOfMonth" 
                                    :key="day"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedDaysOfMonth.includes(day) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 38px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.3rem;"
                                    @click="toggleDayOfMonth(day)">
                                {{ day }}
                            </button>
                        </div>
                    </div>

                    <!-- Hours -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">Hours</h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="hour in hours" 
                                    :key="hour"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedHours.includes(hour) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleHour(hour)">
                                {{ hour }}
                            </button>
                        </div>
                    </div>

                    <!-- Minutes -->
                    <div class="mb-2">
                        <h6 class="mb-1 text-dark" style="font-size: 0.85rem;">
                            Minutes 
                            <span class="text-muted" style="font-size: 0.75rem;">(Common)</span>
                            <button type="button" class="btn btn-sm btn-link text-primary p-0 ms-1" @click="showAllMinutes = !showAllMinutes" style="font-size: 0.7rem;">
                                <i class="fa-solid fa-xs" :class="showAllMinutes ? 'fa-minus' : 'fa-plus'"></i>
                            </button>
                        </h6>
                        <div class="d-flex gap-1 flex-wrap">
                            <button type="button" 
                                    v-for="minute in (showAllMinutes ? allMinutes : commonMinutes)" 
                                    :key="minute"
                                    class="btn btn-sm cron-number-btn"
                                    :class="selectedMinutes.includes(minute) ? 'btn-primary' : 'btn-outline-secondary'"
                                    style="min-width: 42px; border-radius: 3px; font-size: 0.8rem; padding: 0.25rem 0.35rem;"
                                    @click="toggleMinute(minute)">
                                {{ minute }}
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Result Display -->
                <div class="alert alert-light border shadow-sm mb-0 py-1 px-2" style="border-radius: 4px;">
                    <div class="mb-1">
                        <strong style="font-size: 0.8rem;">{{ descriptionText }}</strong>
                    </div>
                    <code class="text-primary" style="font-size: 0.75rem;">{{ cronExpression }}</code>
                </div>
            </div>
            
            <div class="card-footer bg-light border-0 py-2 px-2">
                <div class="d-flex gap-2 justify-content-end">
                    <button type="button" class="btn btn-sm btn-secondary" @click="cancel" style="font-size: 0.8rem;">Cancel</button>
                    <button type="button" class="btn btn-sm btn-primary" @click="confirmSchedule" style="font-size: 0.8rem;">
                        <i class="fa-solid fa-check me-1"></i>Set Schedule
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
let _this = {
    cid: "",
    c: null,
    inputs: {},
    scheduleType: "minutely",
    
    // Minutely configuration
    minuteInterval: 10,
    commonIntervals: [1, 5, 10, 15, 30],
    
    // Selection arrays - use string values to match hours/minutes arrays
    selectedDays: [0],
    selectedHours: ['00'],
    selectedMinutes: ['00'],
    selectedDaysOfMonth: ['01'],
    selectedMonths: [1],
    
    // Display options
    showAllMinutes: false,
    
    // Custom cron
    customCronExpression: "*/10 * * * *",
    
    // Constants
    scheduleTypes: [
        { value: 'minutely', label: 'Minutely' },
        { value: 'hourly', label: 'Hourly' },
        { value: 'daily', label: 'Daily' },
        { value: 'weekly', label: 'Weekly' },
        { value: 'monthly', label: 'Monthly' },
        { value: 'yearly', label: 'Yearly' }
    ],
    daysOfWeek: [
        { value: 0, label: 'Sun', isWeekend: true },
        { value: 1, label: 'Mon', isWeekend: false },
        { value: 2, label: 'Tue', isWeekend: false },
        { value: 3, label: 'Wed', isWeekend: false },
        { value: 4, label: 'Thu', isWeekend: false },
        { value: 5, label: 'Fri', isWeekend: false },
        { value: 6, label: 'Sat', isWeekend: true }
    ],
    months: [
        { value: 1, label: 'Jan' },
        { value: 2, label: 'Feb' },
        { value: 3, label: 'Mar' },
        { value: 4, label: 'Apr' },
        { value: 5, label: 'May' },
        { value: 6, label: 'Jun' },
        { value: 7, label: 'Jul' },
        { value: 8, label: 'Aug' },
        { value: 9, label: 'Sep' },
        { value: 10, label: 'Oct' },
        { value: 11, label: 'Nov' },
        { value: 12, label: 'Dec' }
    ],
    hours: ['00', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', 
            '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23'],
    commonMinutes: ['00', '05', '10', '15', '20', '25', '30', '35', '40', '45', '50', '55'],
    allMinutes: ['00', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', 
                 '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', '31',
                 '32', '33', '34', '35', '36', '37', '38', '39', '40', '41', '42', '43', '44', '45', '46', '47',
                 '48', '49', '50', '51', '52', '53', '54', '55', '56', '57', '58', '59'],
    daysOfMonth: Array.from({ length: 31 }, (_, i) => (i + 1).toString().padStart(2, '0'))
};

export default {
    props: {
        cid: String,
        modelValue: String
    },
    emits: ['update:modelValue'],
    setup(props) {
        _this.cid = props.cid;
        _this.inputs = shared["params_" + _this.cid] || {};
        if (_this.inputs.cronExpression) {
            _this.customCronExpression = _this.inputs.cronExpression;
            parseCronExpression(_this.inputs.cronExpression);
        } else if (props.modelValue) {
            _this.customCronExpression = props.modelValue;
            parseCronExpression(props.modelValue);
        }
    },
    data() {
        return _this;
    },
    computed: {
        cronExpression() {
            if (this.scheduleType === 'minutely') {
                return `*/${this.minuteInterval} * * * *`;
            }
            
            let minute = this.selectedMinutes.length > 0 ? this.selectedMinutes.join(',') : '*';
            let hour = this.selectedHours.length > 0 ? this.selectedHours.join(',') : '*';
            let dayOfMonth = '*';
            let month = '*';
            let dayOfWeek = '*';
            
            if (this.scheduleType === 'hourly') {
                hour = '*';
            } else if (this.scheduleType === 'daily') {
                dayOfWeek = '*';
            } else if (this.scheduleType === 'weekly') {
                dayOfWeek = this.selectedDays.length > 0 ? this.selectedDays.join(',') : '*';
            } else if (this.scheduleType === 'monthly') {
                dayOfMonth = this.selectedDaysOfMonth.length > 0 ? this.selectedDaysOfMonth.join(',') : '*';
                dayOfWeek = '?';
            } else if (this.scheduleType === 'yearly') {
                dayOfMonth = this.selectedDaysOfMonth.length > 0 ? this.selectedDaysOfMonth.join(',') : '*';
                month = this.selectedMonths.length > 0 ? this.selectedMonths.join(',') : '*';
                dayOfWeek = '?';
            }
            
            return `${minute} ${hour} ${dayOfMonth} ${month} ${dayOfWeek}`;
        },
        descriptionText() {
            if (this.scheduleType === 'minutely') {
                return `Run every ${this.minuteInterval} minute${this.minuteInterval > 1 ? 's' : ''}`;
            }
            
            let parts = [];
            
            // Time part
            if (this.selectedMinutes.length === 0 && this.selectedHours.length === 0) {
                parts.push('Every minute');
            } else if (this.selectedHours.length === 0) {
                if (this.selectedMinutes.length === 1) {
                    parts.push(`At ${this.selectedMinutes[0]} minutes past every hour`);
                } else {
                    parts.push(`At minutes ${this.selectedMinutes.join(', ')}`);
                }
            } else {
                let timeStr = this.selectedHours.map(h => {
                    if (this.selectedMinutes.length === 1) {
                        return `${h}:${this.selectedMinutes[0]}`;
                    }
                    return h + ':XX';
                }).join(', ');
                parts.push(`At ${timeStr}`);
            }
            
            // Frequency part
            if (this.scheduleType === 'hourly') {
                parts.push('every hour');
            } else if (this.scheduleType === 'daily') {
                parts.push('every day');
            } else if (this.scheduleType === 'weekly') {
                if (this.selectedDays.length > 0) {
                    let dayNames = this.selectedDays.map(d => this.daysOfWeek.find(day => day.value === d).label);
                    parts.push(`only on ${dayNames.join(', ')}`);
                }
            } else if (this.scheduleType === 'monthly') {
                if (this.selectedDaysOfMonth.length > 0) {
                    parts.push(`on day(s) ${this.selectedDaysOfMonth.join(', ')} of every month`);
                }
            } else if (this.scheduleType === 'yearly') {
                if (this.selectedMonths.length > 0 && this.selectedDaysOfMonth.length > 0) {
                    let monthNames = this.selectedMonths.map(m => this.months.find(mon => mon.value === m).label);
                    parts.push(`on day(s) ${this.selectedDaysOfMonth.join(', ')} of ${monthNames.join(', ')}`);
                }
            }
            
            return parts.join(', ');
        }
    },
    created() {
        _this.c = this;
    },
    mounted() {
        initVueComponent(_this);
    },
    methods: {
        changeScheduleType(type) {
            this.scheduleType = type;
            // Reset selections based on type
            if (type === 'minutely') {
                this.minuteInterval = 10;
            } else if (type === 'hourly') {
                this.selectedHours = [];
                this.selectedMinutes = ['00'];
            } else if (type === 'daily') {
                this.selectedHours = ['00'];
                this.selectedMinutes = ['00'];
            } else if (type === 'weekly') {
                this.selectedDays = [0];
                this.selectedHours = ['00'];
                this.selectedMinutes = ['00'];
            } else if (type === 'monthly') {
                this.selectedDaysOfMonth = ['01'];
                this.selectedHours = ['00'];
                this.selectedMinutes = ['00'];
            } else if (type === 'yearly') {
                this.selectedMonths = [1];
                this.selectedDaysOfMonth = ['01'];
                this.selectedHours = ['00'];
                this.selectedMinutes = ['00'];
            }
        },
        toggleDay(day) {
            const index = this.selectedDays.indexOf(day);
            if (index > -1) {
                this.selectedDays.splice(index, 1);
            } else {
                this.selectedDays.push(day);
            }
            this.selectedDays.sort((a, b) => a - b);
        },
        toggleHour(hour) {
            const index = this.selectedHours.indexOf(hour);
            if (index > -1) {
                this.selectedHours.splice(index, 1);
            } else {
                this.selectedHours.push(hour);
            }
            this.selectedHours.sort();
        },
        toggleMinute(minute) {
            const index = this.selectedMinutes.indexOf(minute);
            if (index > -1) {
                this.selectedMinutes.splice(index, 1);
            } else {
                this.selectedMinutes.push(minute);
            }
            this.selectedMinutes.sort();
        },
        toggleDayOfMonth(day) {
            const index = this.selectedDaysOfMonth.indexOf(day);
            if (index > -1) {
                this.selectedDaysOfMonth.splice(index, 1);
            } else {
                this.selectedDaysOfMonth.push(day);
            }
            this.selectedDaysOfMonth.sort();
        },
        toggleMonth(month) {
            const index = this.selectedMonths.indexOf(month);
            if (index > -1) {
                this.selectedMonths.splice(index, 1);
            } else {
                this.selectedMonths.push(month);
            }
            this.selectedMonths.sort((a, b) => a - b);
        },
        confirmSchedule() {
            if (this.inputs.callback) {
                this.inputs.callback(this.cronExpression);
            }
            
            this.$emit('update:modelValue', this.cronExpression);
            this.close();
        },
        cancel() {
            this.close();
        },
        close() {
            shared.closeComponent(_this.cid);
        }
    }
}

function parseCronExpression(expression) {
    // Basic parser to pre-select values from existing cron expression
    const parts = expression.split(' ');
    if (parts.length !== 5) return;
    
    const [minute, hour, dayOfMonth, month, dayOfWeek] = parts;
    
    // Check for minutely pattern (*/N)
    if (minute.startsWith('*/') && hour === '*' && dayOfMonth === '*' && month === '*' && dayOfWeek === '*') {
        _this.scheduleType = 'minutely';
        _this.minuteInterval = parseInt(minute.substring(2));
        return;
    }
    
    // Determine schedule type
    if (dayOfWeek !== '*' && dayOfWeek !== '?') {
        _this.scheduleType = 'weekly';
        if (dayOfWeek !== '*') {
            _this.selectedDays = dayOfWeek.split(',').map(d => parseInt(d));
        }
    } else if (dayOfMonth !== '*' && dayOfMonth !== '?') {
        if (month !== '*') {
            _this.scheduleType = 'yearly';
            _this.selectedMonths = month.split(',').map(m => parseInt(m));
        } else {
            _this.scheduleType = 'monthly';
        }
        _this.selectedDaysOfMonth = dayOfMonth.split(',');
    } else if (hour === '*') {
        _this.scheduleType = 'hourly';
    } else {
        _this.scheduleType = 'daily';
    }
    
    // Parse hour - keep as string
    if (hour !== '*') {
        _this.selectedHours = hour.split(',').map(h => h.padStart(2, '0'));
    } else {
        _this.selectedHours = [];
    }
    
    // Parse minute - keep as string
    if (minute !== '*' && !minute.startsWith('*/')) {
        _this.selectedMinutes = minute.split(',').map(m => m.padStart(2, '0'));
    } else if (_this.scheduleType !== 'minutely') {
        _this.selectedMinutes = _this.scheduleType === 'hourly' ? ['00'] : [];
    }
}
</script>

<style scoped>
/* Override Bootstrap primary buttons to use theme colors - SAME for all buttons */
:deep(.btn-primary) {
    background-color: var(--bs-primary) !important;
    border-color: var(--bs-primary) !important;
    color: white !important;
}

:deep(.btn-primary:hover),
:deep(.btn-primary:focus) {
    background-color: var(--bs-primary-dark) !important;
    border-color: var(--bs-primary-dark) !important;
    color: white !important;
}

:deep(.btn-outline-primary) {
    color: var(--bs-primary) !important;
    border-color: var(--bs-primary) !important;
    background-color: transparent !important;
}

:deep(.btn-outline-primary:hover),
:deep(.btn-outline-primary:focus) {
    background-color: var(--bs-primary) !important;
    border-color: var(--bs-primary) !important;
    color: white !important;
}

:deep(.text-primary) {
    color: var(--bs-primary) !important;
}

/* Fix button text alignment */
:deep(.btn-group .btn) {
    display: flex;
    align-items: center;
    justify-content: center;
    text-align: center;
}

/* Secondary buttons - neutral gray with better text color */
.btn-outline-secondary {
    color: #6c757d;
    border-color: #dee2e6;
    background-color: white;
}

.btn-outline-secondary:hover,
.btn-outline-secondary:focus {
    background-color: #e9ecef;
    border-color: #dee2e6;
    color: #495057;
}

.btn-outline-secondary.text-danger {
    color: #dc3545;
}

.btn-outline-secondary.text-danger:hover,
.btn-outline-secondary.text-danger:focus {
    background-color: #e9ecef;
    border-color: #dee2e6;
    color: #dc3545;
}
</style>
