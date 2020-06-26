<template>
    <b-container class="pt-2" v-if="rights.isConnected">
        <h2 class="pb-3 text-center">Events Management</h2>
        <b-table-simple class="table" table-variant="dark" hover small responsive="lg">
            <b-thead head-variant="dark">
                <b-tr class="d-flex w-100">
                    <b-th :class="(rights.isSecretary || rights.isAdmin) ? 'col-3' : 'col-4'" class="text-center">
                        Type
                    </b-th>
                    <b-th class="col-4 text-center">
                        From
                    </b-th>
                    <b-th class="col-4 text-center">
                        To
                    </b-th>
                    <b-th class="col-1" v-if="rights.isSecretary || rights.isAdmin">
                        <b-btn size="sm" variant="success" v-if="!addMode || edit !== -1" v-on:click="setAddMode()">
                            <font-awesome-icon class="mx-2" icon="plus"></font-awesome-icon>
                        </b-btn>
                    </b-th>
                </b-tr>
            </b-thead>
            <b-tbody>
                <b-tr class="d-flex" v-if="addMode">
                    <b-td class="col-3">
                        <div v-if="newType">
                            <b-form-input
                                    size="sm"
                                    type="text"
                                    v-model="newEvent.eventType.label"
                                    class="w-75 d-inline-block"
                            ></b-form-input>
                            <font-awesome-icon
                                    class="mx-1"
                                    icon="times"
                                    v-on:click="unsetAddNewTypeMode()"
                            ></font-awesome-icon>
                        </div>
                        <div v-else>
                            <b-form-select
                                    size="sm"
                                    v-model="newEvent.eventType.id"
                                    :options="eventTypes"
                                    value-field="id"
                                    text-field="label"
                                    class="w-75"
                            ></b-form-select>
                            <font-awesome-icon
                                    class="mx-2 plus-hover"
                                    icon="plus"
                                    v-on:click="setAddNewTypeMode()"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    :class="{ disabled: !dropEventTypeEnabled}"
                                    class="ml-2"
                                    icon="trash-alt"
                                    v-on:click="dropType(newEvent.eventType.id)"
                            ></font-awesome-icon>
                        </div>
                    </b-td>
                    <b-td class="col-3 px-0">
                        <b-form-datepicker
                                dark
                                size="sm"
                                :min="minDate"
                                :max="maxDate"
                                v-on:input="computedDates()"
                                v-model="newEvent.date.startDate"
                        ></b-form-datepicker>
                    </b-td>
                    <b-td class="col-1 px-0">
                        <b-form-timepicker
                                dark
                                no-close-button
                                hide-header
                                size="sm"
                                v-on:input="computedDates()"
                                v-model="newEvent.date.startTime"
                        ></b-form-timepicker>
                    </b-td>
                    <b-td class="col-3 px-0">
                        <b-form-datepicker
                                dark
                                size="sm"
                                :min="minDate"
                                :max="maxDate"
                                v-on:input="computedDates()"
                                v-model="newEvent.date.endDate"
                        ></b-form-datepicker>
                    </b-td>
                    <b-td class="col-1 px-0">
                        <b-form-timepicker
                                no-close-button
                                hide-header
                                size="sm"
                                v-on:input="computedDates()"
                                v-model="newEvent.date.endTime"
                        ></b-form-timepicker>
                    </b-td>
                    <b-td class="col-1">
                        <font-awesome-icon
                                :class="{ disabled : !insertEnabled }"
                                class="mx-1"
                                icon="check"
                                v-on:click="insert()"
                        ></font-awesome-icon>
                        <font-awesome-icon
                                class="mx-1"
                                icon="times"
                                v-on:click="unsetAddMode()"
                        ></font-awesome-icon>
                    </b-td>
                </b-tr>
                <b-tr
                        class="d-flex"
                        v-for="event in events"
                        :key="event.id"
                >
                    <b-td :class="rights.isSecretary || rights.isAdmin ? 'col-3' : 'col-4'" class="text-center">
                        <span v-if="edit !== event.id">{{ event.eventType.label }}</span>
                        <b-form-select
                                v-else
                                size="sm"
                                v-model="event.eventType.id"
                                :options="eventTypes"
                                value-field="id"
                                text-field="label"
                        ></b-form-select>

                    </b-td>
                    <b-td class="col-4 text-center" v-if="edit !== event.id">
                        <span >{{ new Date(event.startDate).toString().substr(0,21) }}</span>
                    </b-td>
                    <b-td class="col-3 px-0" v-if="edit === event.id">
                        <b-form-datepicker
                                dark
                                size="sm"
                                :min="minDate"
                                :max="maxDate"
                                v-on:input="computedDates()"
                                v-model="event.date.startDate"
                        ></b-form-datepicker>
                    </b-td>
                    <b-td class="col-1 px-0" v-if="edit === event.id">
                        <b-form-timepicker
                                size="sm"
                                no-close-button
                                hide-header
                                v-on:input="computedDates()"
                                v-model="event.date.startTime"
                        ></b-form-timepicker>
                    </b-td>

                    <b-td class="col-4 text-center" v-if="edit !== event.id">
                        <span>{{ new Date(event.endDate).toString().substr(0,21) }}</span>
                    </b-td>
                        <b-td class="col-3 px-0" v-if="edit === event.id">
                            <b-form-datepicker
                                    dark
                                    size="sm"
                                    :min="minDate"
                                    :max="maxDate"
                                    v-on:input="computedDates()"
                                    v-model="event.date.endDate"
                            ></b-form-datepicker>
                        </b-td>
                        <b-td class="col-1 px-0" v-if="edit === event.id">
                            <b-form-timepicker
                                    no-close-button
                                    hide-header
                                    size="sm"
                                    v-on:input="computedDates()"
                                    v-model="event.date.endTime"
                            ></b-form-timepicker>
                        </b-td>
                    <b-td v-if="rights.isSecretary || rights.isAdmin" class="col-1">
                        <font-awesome-icon
                                v-if="edit !== event.id"
                                class="mx-2"
                                icon="edit"
                                v-on:click="editMode(event.id)"
                        ></font-awesome-icon>
                        <div v-else>
                            <font-awesome-icon
                                    :class="{ disabled: !updateEnabled}"
                                    class="mx-1"
                                    icon="check"
                                    v-on:click="update(event)"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    class="mx-1"
                                    icon="times"
                                    v-on:click="exitEdit(true)"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    class="ml-2"
                                    icon="trash-alt"
                                    v-on:click="drop(event)"
                            ></font-awesome-icon>
                        </div>
                    </b-td>
                </b-tr>
            </b-tbody>
        </b-table-simple>
    </b-container>
    <b-container class="pt-2" v-else>
        <em class="d-block mx-auto pb-3 text-center">401 Unauthorized - Access can not be granted</em>
    </b-container>
</template>

<script>
    // TODO import axios from "axios"

    export default {
        name: "Event",
        props: {
            baseUrl: String,
            rights: Object
        },
        mounted: function() {
            this.getAll();
            this.getAllEventTypes();
        },
        data: function () {
            return {
                events: [],
                eventTypes: [],
                newEvent: undefined,
                edit: -1,
                newType: false,
                savedEvent: undefined,
                addMode: false
            }
        },
        methods: {
            getAll: function () {
                // TODO
                // let that = this;
                // axios.get(this.baseUrl + '/calendar', { withCredentials: true })
                //     .then(function (response) {
                //         that.items = response.data;
                //     }).catch(error => {
                //     that.$emit('error', { title : "Error while retrieving event list", error : error })
                // });
                this.events = [
                    {
                        id: 1,
                        startDate: '2020-06-29T17:00:00',
                        endDate: '2020-06-29T20:00:00',
                        calendarID: 1,
                        eventType: {
                            id: 1,
                            label: 'Training'
                        }
                    },
                    {
                        id: 2,
                        startDate: '2020-06-29T20:00:00',
                        endDate: '2020-06-29T23:00:00',
                        calendarID: 1,
                        eventType: {
                            id: 2,
                            label: 'Show'
                        }
                    }
                ]
            },
            getAllEventTypes: function () {
                // TODO
                // let that = this;
                // axios.get(this.baseUrl + '/calendar/types', { withCredentials: true })
                //     .then(function (response) {
                //         that.eventTypes = response.data
                //     }).catch(error => {
                //     that.$emit('error', { title : "Error while retrieving event type list", error : error })
                // });
                this.eventTypes = [
                    {
                        id: 1,
                        label: 'Training'
                    },
                    {
                        id: 2,
                        label: 'Show'
                    },
                    {
                        id: 3,
                        label: 'Private Session'
                    }
                ]
            },
            editMode: function (idEvent) {
                if (this.addMode) {
                    this.unsetAddMode();
                }
                if (this.savedEvent !== undefined) {
                    this.exitEdit(true);
                }
                let toSaveEvent = this.events.find(event => event.id === idEvent);
                this.savedEvent = {
                    id: idEvent,
                    startDate: toSaveEvent.startDate,
                    endDate: toSaveEvent.endDate,
                    calendarID: toSaveEvent.calendarID,
                    eventType: {
                        id: toSaveEvent.eventType.id,
                        label: toSaveEvent.eventType.label
                    }
                }
                toSaveEvent.date = {
                    startDate: toSaveEvent.startDate.substr(0, 10),
                    startTime: toSaveEvent.startDate.substr(11, 5),
                    endDate: toSaveEvent.endDate.substr(0, 10),
                    endTime: toSaveEvent.endDate.substr(11, 5),
                }
                this.edit = idEvent;
            },
            exitEdit: function (rollback) {
                if (rollback) {
                    let index = this.events.findIndex(event => event.id === this.savedEvent.id);
                    this.events[index] = this.savedEvent;
                }
                this.savedEvent = undefined;
                this.edit = -1;
            },
            update: function (event) {
                if (!this.updateEnabled) {
                    return;
                }
                // TODO
                // let that = this;
                // let updateData = {
                //     startDate: event.startDate,
                //     endDate: event.endDate,
                //     calendarID: event.calendarID,
                //     eventType: event.eventType
                // };
                // axios.post(this.baseUrl + '/calendar/' + event.id, updateData,{ withCredentials: true })
                //     .then(() => { that.exitEdit(false); })
                //     .catch(error => {
                //         that.$emit('error', { title : "Error while updating event", error : error });
                //     })
                // ;
                let index = this.events.findIndex(e => e.id === event.id);
                this.events[index] = event;
                this.exitEdit(false);
            },
            insert: function() {
                if (!this.insertEnabled) {
                    return;
                }
                // TODO
                // let that = this;
                // let insertData = {
                //     startDate: this.newEvent.startDate,
                //     endDate: this.newEvent.endDate,
                //     calendarID: this.newEvent.calendarID,
                //     eventType: this.newEvent.eventType
                // }
                // axios.post(this.baseUrl + '/calendar/', insertData, { withCredentials: true })
                //     .then(() => {
                //         that.getAll();
                //         that.getAllItemTypes();
                //         that.unsetAddMode();
                //     })
                //     .catch(error => {
                //         that.$emit('error', { title : "Error while inserting new event", error : error });
                //     })
                // ;
                this.events.push(this.newEvent);
                this.unsetAddMode();
            },
            drop: function (droppedEvent) {
                // TODO
                // let that = this;
                // axios.delete(
                //     this.baseUrl + '/calendar/' + droppedEvent.id,
                //     { withCredentials: true }
                // ).then(() => {
                //     that.exitEdit(false);
                //     that.events.splice(
                //         that.events.findIndex(event => event.id === droppedEvent.id)
                //         ,1
                //     )
                // }).catch(error => {
                //     that.$emit('error', { title : "Error while deleting event", error : error })
                // });
                this.exitEdit(false);
                this.events.splice(
                    this.events.findIndex(event => event.id === droppedEvent.id)
                    ,1
                )
            },
            dropType: function (idType) {
                if (!this.dropEventTypeEnabled) {
                    return;
                }
                // TODO
                // if (this.eventTypes[0] === undefined || idType <= 0 ){
                //     return;
                // }
                // let that = this;
                // axios.delete(this.baseUrl + '/stock/items/' + idType, { withCredentials: true })
                //     .then(() => {
                //         that.eventTypes.splice(that.eventTypes.findIndex(type => type.id === idType), 1)
                //         that.newEvent.eventType.id = that.eventTypes[0] !== undefined ? that.eventTypes[0].id : 0;
                //     }).catch(error => {
                //     that.$emit('error', {title: "Error while deleting event type", error: error})
                // });
                this.eventTypes.splice(this.eventTypes.findIndex(type => type.id === idType), 1)
                this.newEvent.eventType.id = this.eventTypes[0] !== undefined ? this.eventTypes[0].id : 0;
            },
            setAddMode: function () {
                if (this.edit !== -1) {
                    this.exitEdit(true);
                }
                this.newEvent = {
                    id: 0,
                    startDate: (new Date()).toJSON().substr(0,19),
                    endDate: (new Date(Date.now() + 3.6e+6)).toJSON().substr(0,19),
                    calendarID: 1,
                    eventType: {
                        id: this.eventTypes[0] !== undefined ? this.eventTypes[0].id : 0,
                        label: this.eventTypes[0] !== undefined ? this.eventTypes[0].label : ""
                    }
                };
                this.newEvent.date = {
                    startDate: this.newEvent.startDate.substr(0, 10),
                    startTime: this.newEvent.startDate.substr(11, 5),
                    endDate: this.newEvent.endDate.substr(0, 10),
                    endTime: this.newEvent.endDate.substr(11, 5),
                };
                this.addMode = true;
                this.computedDates();
            },
            unsetAddMode: function () {
                this.addMode = false;
                this.newType = false;
                this.newEvent = undefined;
            },
            setAddNewTypeMode: function () {
                this.newType = true;
                this.newEvent.eventType.id = 0;
                this.newEvent.eventType.label = "";
            },
            unsetAddNewTypeMode: function () {
                this.newType = false;
                this.newEvent.eventType.label =
                    this.eventTypes[0] !== undefined ? this.eventTypes[0].label : "";
                this.newEvent.eventType.id = this.eventTypes[0] !== undefined ? this.eventTypes[0].id : 0;
            },
            computedDates: function () {
                const event = this.edit === -1 ? this.newEvent : this.events[this.edit];
                event.startDate = event.date.startDate + 'T' + event.date.startTime;
                event.endDate = event.date.endDate + 'T' + event.date.endTime;
            }
        },
        computed: {
            insertEnabled: function () {
                if (this.newEvent === undefined) {
                    return false;
                }
                if (this.newEvent.eventType.label === '' && this.newEvent.eventType.id === 0) {
                    return false;
                }
                if (this.newEvent.startDate >= this.newEvent.endDate) {
                    return false;
                }
                return !(this.newEvent.eventType.id === 0
                    && (this.newEvent.eventType.label === ""
                        || this.eventTypes.find(
                            type => type.label.toLowerCase() === this.newEvent.eventType.label.trim().toLowerCase()
                        ) !== undefined
                    )
                );
            },
            updateEnabled: function () {
                if (this.savedEvent === undefined) {
                    return false;
                }
                let event = this.events.find(event => event.id === this.edit);
                return event.startDate < event.endDate && (
                    event.startDate !== this.savedEvent.startDate
                    || event.endDate !== this.savedEvent.endDate
                    || event.eventType.id !== this.savedEvent.eventType.id
                );
            },
            minDate: function () {
                const now = new Date();
                return new Date(now.getFullYear(), now.getMonth(), now.getDate());
            },
            maxDate: function () {
                const now = new Date();
                return new Date(now.getFullYear() + 1, now.getMonth(), now.getDate());
            },
            dropEventTypeEnabled: function () {
                if (this.newEvent === undefined) {
                    return false;
                }
                if (this.newEvent.eventType.id === 0) {
                    return false;
                }
                return this.events.find(event => event.eventType.id === this.newEvent.eventType.id) === undefined;
            }
        }
    }
</script>

<style lang="scss" scoped>
    tbody, thead {
        svg {
            &:hover:not(.disabled)  {
                cursor: pointer;
                &.fa-edit, &.fa-file-download {
                    path {
                        color: rgba(0, 123, 255, 0.8);
                    }
                }
            }
            &.fa-check, &.plus-hover {
                &:not(.disabled) {
                    path {
                        color: rgba(40, 167, 69, 0.6)
                    }
                    &:hover {
                        path {
                            color: rgba(40, 167, 69, 1)
                        }
                    }
                }
                &.disabled {
                    path {
                        color: rgba(108, 117, 125, 0.8);
                    }
                }

            }
            &.fa-times, &.fa-trash-alt {
                &:not(.disabled) {
                    path {
                        color: rgba(220, 53, 69, 0.6);
                    }
                    &:hover {
                        path {
                            color: rgba(220, 53, 69, 1);
                        }
                    }
                }
                &.disabled {
                    path {
                        color: rgba(108, 117, 125, 0.8);
                    }
                }
            }
        }
    }
    .form-control, .custom-select {
        bdi, .b-calendar-grid-caption {
            color: #495057 !important;
        }
        .b-calendar-grid-weekdays {
            small {
                color: #495057 !important;
            }
        }
        background-color: rgba(52, 58, 64, 0.8) !important;
        border-color: #454d55 !important;
        color: rgba(255, 255, 255, 0.8) !important;


    }
</style>