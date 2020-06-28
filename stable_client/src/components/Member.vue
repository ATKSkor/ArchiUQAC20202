<template>
    <b-container class="pt-2" v-if="rights.isSecretary || rights.isAdmin">
        <h2 class="pb-3 text-center">Member Management</h2>
        <b-table-simple class="table" table-variant="dark" hover small responsive="sm">
            <b-thead head-variant="dark">
                <b-tr class="d-flex w-100">
                    <b-th class="col-5 text-center">Last name</b-th>
                    <b-th class="col-5 text-center">First name</b-th>
                    <b-th class="col-2">
                        <b-btn size="sm" variant="success" v-if="!addMode || edit !== -1" v-on:click="setAddMode()">
                            <font-awesome-icon class="mx-2" icon="plus"></font-awesome-icon>
                            New
                        </b-btn>
                    </b-th>
                </b-tr>
            </b-thead>
            <b-tbody>
                <b-tr class="d-flex" v-if="addMode">
                    <b-td class="col-5">
                        <b-form-input
                                size="sm"
                                type="text"
                                v-model="newMember.name"
                        ></b-form-input>
                    </b-td>
                    <b-td class="col-5">
                        <b-form-input
                                size="sm"
                                type="text"
                                v-model="newMember.surname"
                        ></b-form-input>
                    </b-td>
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
                        v-for="member in owners"
                        :key="member.id"
                >
                    <b-td  class="col-5 text-center">
                        <span v-if="edit !== member.id">{{ member.name }}</span>
                        <b-form-input
                                v-else
                                size="sm"
                                type="text"
                                v-model="member.name"
                                autocomplete="off"
                        ></b-form-input>
                    </b-td>
                    <b-td class="col-5 text-center">
                        <span v-if="edit !== member.id">{{ member.surname }}</span>
                        <b-form-input
                                v-else
                                size="sm"
                                type="text"
                                v-model="member.surname"
                                autocomplete="off"
                        ></b-form-input>
                    </b-td>
                    <b-td class="col-2">
                        <font-awesome-icon
                                v-if="edit !== member.id"
                                class="mx-2"
                                icon="edit"
                                v-on:click="editMode(member.id)"
                        ></font-awesome-icon>
                        <div v-else>
                            <font-awesome-icon
                                    :class="{ disabled: !updateEnabled}"
                                    class="mx-1"
                                    icon="check"
                                    v-on:click="update(member)"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    class="mx-1"
                                    icon="times"
                                    v-on:click="exitEdit(true)"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    :class="{disabled : !dropEnabled}"
                                    class="ml-2"
                                    icon="trash-alt"
                                    v-on:click="drop(member)"
                            ></font-awesome-icon>
                        </div>
                    </b-td>
                </b-tr>
            </b-tbody>
        </b-table-simple>
        <h4 class="text-center" v-if="owners === undefined || owners.length === 0">No member found</h4>
    </b-container>
    <b-container class="pt-2" v-else>
        <em class="d-block mx-auto pb-3 text-center">401 Unauthorized - Access can not be granted</em>
    </b-container>
</template>

<script>
    import axios from "axios";

    export default {
        name: "Member",
        mounted: function() {
            this.$emit('refresh-owner');
        },
        props: {
            baseUrl: String,
            rights: Object,
            owners: Array,
        },
        data: function () {
            return {
                edit: -1,
                savedMember: undefined,
                addMode: false,
                newMember: undefined
            }
        },
        computed: {
            insertEnabled: function () {
                return this.newMember.name !== '' && this.newMember.surname !== '';
            },
            updateEnabled: function () {
                if (this.savedMember === undefined) {
                    return false;
                }
                let member = this.owners.find(p => p.id === this.savedMember.id);
                return member.name !== this.savedMember.name || member.surname !== this.savedMember.surname;
            },
            dropEnabled: function () {
                const watchedMember = this.edit !== -1 ? this.owners.find(o => o.id === this.edit) : this.savedMember;
                return watchedMember.horsesIDs === undefined || watchedMember.horsesIDs.length === 0;
            }
        },
        methods: {
            editMode: function (id) {
                if (this.addMode) {
                    this.unsetAddMode();
                }
                if (this.edit !== -1) {
                    this.exitEdit(true);
                }
                const toSaveMember = this.owners.find(p => p.id === id);
                this.savedMember = {
                    id: toSaveMember.id,
                    name: toSaveMember.name,
                    surname: toSaveMember.surname
                }
                this.edit = id;
            },
            exitEdit: function (rollback) {
                if (rollback) {
                    this.$emit('refresh-owner');
                }
                this.edit = -1;
                this.savedMember = undefined;
            },
            update: function (member) {
                if (!this.updateEnabled) {
                    return;
                }
                let that = this;
                let updateData = {
                    name : member.name,
                    surname: member.surname
                };
                axios.post(this.baseUrl + '/person/' + member.id, updateData, { withCredentials: true })
                    .then(() => {
                        this.$emit('refresh-owner');
                        that.exitEdit(false);
                    })
                    .catch(error => {
                        that.$emit('error', { title : "Error while updating person entry", error : error });
                    })
                ;
            },
            insert: function() {
                if (!this.insertEnabled) {
                    return;
                }
                let that = this
                let insertData = {
                    name: this.newHorse.name,
                    boxID: this.newHorse.boxID,
                    ownerId: this.newHorse.ownerID
                };
                axios.post(this.baseUrl + '/person/', insertData, { withCredentials: true })
                    .then(() => {
                        that.$emit('refresh-owner');
                        that.unsetAddMode();
                    })
                    .catch(error => {
                        that.$emit('error', { title : "Error while inserting new person entry", error : error });
                    })
                ;
            },
            drop: function (droppedPerson) {
                if (!this.dropEnabled) {
                    return;
                }
                let that = this;
                axios.delete(this.baseUrl + '/person/' + droppedPerson.id, { withCredentials: true } )
                    .then(() => {
                        that.$emit('refresh-owner');
                        that.exitEdit(false);
                    }).catch(error => {
                    that.$emit('error', { title : "Error while deleting member entry", error : error })
                })
            },
            setAddMode: function () {
                if (this.edit !== -1) {
                    this.exitEdit(true);
                }
                this.newMember = {
                    id: 0,
                    name: "",
                    surname: ""
                };
                this.addMode = true;
            },
            unsetAddMode: function () {
                this.addMode = false;
                this.newMember = undefined;
            }
        }
    }
</script>

<style lang="scss" scoped>
    body, thead {
        svg {
            &:hover:not(.disabled)  {
                cursor: pointer;
                &.fa-edit {
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
        background-color: rgba(52, 58, 64, 0.8) !important;
        border-color: #454d55;
        color: rgba(255, 255, 255, 0.8);
    }
</style>