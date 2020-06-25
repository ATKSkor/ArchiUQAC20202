<template>
    <b-container class="pt-2">
        <h2 class="pb-3 text-center">Horse Management</h2>
        <b-table-simple class="table" table-variant="dark" hover small responsive="sm">
            <b-thead head-variant="dark">
                <b-tr class="d-flex w-100">
                    <b-th
                            :class="(rights.isGroom || rights.isAdmin) ? 'col-3' : 'col-4'"
                            class="text-center"
                    >
                        Name
                    </b-th>
                    <b-th class="col-1 text-center">Box #</b-th>
                    <b-th
                            :class="(rights.isGroom || rights.isAdmin) ? 'col-3' : 'col-4'"
                            class="text-center"
                    >
                        Owner
                    </b-th>
                    <b-th class="col-3 text-center">Medic Entries</b-th>
                    <b-th class="col-2" v-if="rights.isGroom || rights.isAdmin">
                        <b-btn size="sm" variant="success"
                               v-if="!addMode || edit !== -1"
                               v-on:click="setAddMode()"
                        >
                            <font-awesome-icon class="mx-2" icon="plus"></font-awesome-icon>
                            New
                        </b-btn>
                    </b-th>
                </b-tr>
            </b-thead>
            <b-tbody>
                <b-tr class="d-flex" v-if="addMode">
                    <b-td :class="(rights.isGroom || rights.isAdmin) ? 'col-3' : 'col-4'">
                        <b-form-input
                                size="sm"
                                type="text"
                                v-model="newHorse.name"
                        ></b-form-input>
                    </b-td>
                    <b-td class="col-1">
                        <b-form-select
                                size="sm"
                                v-model="newHorse.boxID"
                                :options="boxes"
                                value-field="id"
                                text-field="id"
                        ></b-form-select>
                    </b-td>
                    <b-td :class="(rights.isGroom || rights.isAdmin) ? 'col-3' : 'col-4'">
                        <b-form-select
                                size="sm"
                                v-model="newHorse.ownerID"
                                :options="owners"
                                value-field="id"
                                text-field="fullname"
                        ></b-form-select>
                    </b-td>
                    <b-td class="col-3"></b-td>
                    <b-td class="col-2">
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
                        v-for="horse in horses"
                        :key="horse.id"
                >
                    <b-td :class="(rights.isGroom || rights.isAdmin) ? 'col-3' : 'col-4'" class="text-center">
                        <span v-if="edit !== horse.id">{{ horse.name }}</span>
                        <b-form-input
                                v-else
                                size="sm"
                                type="text"
                                v-model="horse.name"
                                autocomplete="off"
                        ></b-form-input>
                    </b-td>
                    <b-td class="col-1 text-center">
                        <span v-if="edit !== horse.id">{{ horse.boxID }}</span>
                        <b-form-select
                                v-else
                                size="sm"
                                v-model="horse.boxID"
                                :options="boxes"
                                value-field="id"
                                text-field="id"
                        ></b-form-select>
                    </b-td>
                    <b-td :class="(rights.isGroom || rights.isAdmin) ? 'col-3' : 'col-4'" class="text-center">
                        <span v-if="edit !== horse.id">{{ horse.ownerFullName }}</span>
                        <b-form-select
                                v-else
                                size="sm"
                                v-model="horse.ownerID"
                                :options="owners"
                                value-field="id"
                                text-field="fullname"
                        ></b-form-select>
                    </b-td>
                    <b-td class="col-3 text-center">
                        {{ horse.medicEntryIDs.length === 0 ? 'none' : horse.medicEntryIDs.length }}
                        <font-awesome-icon
                                class="mx-2"
                                icon="file-download"
                                v-if="horse.medicEntryIDs.length > 0">

                        </font-awesome-icon>
                    </b-td>
                    <b-td class="col-2" v-if="rights.isGroom || rights.isAdmin">
                        <font-awesome-icon
                                v-if="edit !== horse.id"
                                class="mx-2"
                                icon="edit"
                                v-on:click="editMode(horse.id)"
                        ></font-awesome-icon>
                        <div v-else>
                            <font-awesome-icon
                                    :class="{ disabled: !updateEnabled}"
                                    class="mx-1"
                                    icon="check"
                                    v-on:click="update(horse)"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    class="mx-1"
                                    icon="times"
                                    v-on:click="exitEdit(true)"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    class="ml-2"
                                    icon="trash-alt"
                                    v-on:click="drop(horse)"
                            ></font-awesome-icon>
                        </div>
                    </b-td>
                </b-tr>
            </b-tbody>
        </b-table-simple>
        <h4 class="text-center" v-if="horses === undefined || horses.length === 0">No horse found</h4>
    </b-container>
</template>

<script>
    import axios from "axios";

    export default {
        name: "Horse",
        mounted: function() {
            this.getAll();
            if (this.owners.length === 0) {
                this.$emit('refresh-owner');
            }
            if (this.boxes.length === 0) {
                this.$emit('refresh-box')
            }
        },
        props: {
            baseUrl: String,
            rights: Object,
            owners: Array,
            boxes: Array
        },
        data: function () {
            return {
                horses : [],
                edit : -1,
                savedHorse: undefined,
                addMode : false,
                newHorse: undefined
            }
        },
        computed: {
            insertEnabled: function () {
                return this.newHorse.name !== '' && this.newHorse.ownerID > 0;
            },
            updateEnabled: function () {
                if (this.savedHorse === undefined) {
                    return false;
                }
                let horse = this.horses.find(horse => horse.id === this.savedHorse.id);
                return horse.name !== this.savedHorse.name || horse.ownerID !== this.savedHorse.ownerID;
            }
        },
        methods: {
            getAll: function () {
                let that = this;
                axios.get(this.baseUrl + '/horse', { withCredentials: true })
                    .then(function (response) {
                        that.horses = response.data;
                    }).catch(error => {
                    that.$emit('error', { title : "Error while retrieving horses list", error : error })
                })
            },
            editMode: function (id) {
                if (this.addMode) {
                    this.unsetAddMode();
                }
                let toSaveHorse = this.horses.find(horse => horse.id === id);
                this.savedHorse = {
                    boxID: toSaveHorse.boxID,
                    id: toSaveHorse.id,
                    medicEntryIDs: toSaveHorse.medicEntryIDs,
                    name: toSaveHorse.name,
                    ownerFullName: toSaveHorse.ownerFullName,
                    ownerID: toSaveHorse.ownerID
                }
                this.edit = id;
            },
            exitEdit: function (rollback) {
                if (rollback) {
                    let index = this.horses.findIndex(horse => horse.id === this.savedHorse.id);
                    this.horses[index] = this.savedHorse;
                }
                this.edit = -1;
                this.savedHorse = undefined;
            },
            update: function (horse) {
                if (!this.updateEnabled) {
                    return;
                }
                let that = this;
                let updateData = {
                    name : horse.name,
                    ownerId : horse.ownerID
                };
                axios.post(this.baseUrl + '/horse/' + horse.id, updateData, { withCredentials: true })
                    .then(() => {
                        horse.ownerFullName = that.owners.find(owner => owner.id === horse.ownerID).fullname;
                        that.exitEdit(false);
                    })
                    .catch(error => {
                        that.$emit('error', { title : "Error while updating horse entry", error : error });
                    })
                ;
            },
            insert: function() {
                if (!this.insertEnabled) {
                    return;
                }
                let that = this
                let horse = this.newHorse;
                let insertData = {
                    name: horse.name,
                    ownerId: horse.ownerID
                };
                axios.post(this.baseUrl + '/horse/', insertData, { withCredentials: true })
                    .then(() => {
                        that.getAll();
                        that.unsetAddMode();
                    })
                    .catch(error => {
                        that.$emit('error', { title : "Error while inserting new horse entry", error : error });
                    })
                ;
            },
            drop: function (droppedHorse) {
                let that = this;
                axios.delete(this.baseUrl + '/horse/' + droppedHorse.id, { withCredentials: true } )
                    .then(() => {
                        that.exitEdit(false);
                        this.horses.splice(this.horses.findIndex(horse => horse.id === droppedHorse.id), 1)
                    }).catch(error => {
                    that.$emit('error', { title : "Error while deleting horse entry", error : error })
                })
            },
            setAddMode: function () {
                if (this.edit !== -1) {
                    this.exitEdit(true);
                }
                this.newHorse = {
                    id: 0,
                    name: "",
                    ownerID: 1,
                    boxID: 1,
                    ownerFullName: "",
                    medicEntryIDs: []
                };
                this.addMode = true;
            },
            unsetAddMode: function () {
                this.addMode = false;
                this.newHorse = undefined;
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
            &.fa-check {
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
                path {
                    color: rgba(220, 53, 69, 0.6);
                }
                &:hover {
                    path {
                        color: rgba(220, 53, 69, 1);
                    }
                }
            }
        }
    }
    .form-control, .custom-select {
        background-color: rgba(52, 58, 64, 0.8) !important;
        border-color: #454d55;
        color: rgba(255, 255, 255, 0.8);
    }
</style>