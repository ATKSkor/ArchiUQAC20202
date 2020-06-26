<template>
    <b-container class="pt-2" v-if="rights.isAdmin || rights.isSecretary">
        <h2 class="pb-3 text-center">Equipment Management</h2>
        <b-table-simple class="table" table-variant="dark" hover small responsive="sm">
            <b-thead head-variant="dark">
                <b-tr class="d-flex w-100">
                    <b-th class="col-8 text-center">
                        Item
                    </b-th>
                    <b-th class="col-2 text-center">
                        Quantity
                    </b-th>
                    <b-th class="col-2">
                        <b-btn size="sm" variant="success"
                               v-if="!addMode || savedItem !== undefined"
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
                    <b-td class="col-8">
                        <div v-if="newType">
                            <b-form-input
                                    size="sm"
                                    type="text"
                                    v-model="newItem.itemName"
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
                                    v-model="newItem.stockItemId"
                                    :options="availableTypes"
                                    value-field="id"
                                    text-field="itemName"
                                    class="w-75"
                            ></b-form-select>
                            <font-awesome-icon
                                    class="mx-2 plus-hover"
                                    icon="plus"
                                    v-on:click="setAddNewTypeMode()"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    :class="{ disabled: availableTypes[0] === undefined}"
                                    class="ml-2"
                                    icon="trash-alt"
                                    v-on:click="dropType(newItem.stockItemId)"
                            ></font-awesome-icon>
                        </div>


                    </b-td>
                    <b-td class="col-2">
                        <b-form-input
                                size="sm"
                                type="number"
                                min="0"
                                step="1"
                                :state="newItem.quantity >= 0 && newItem.quantity == parseInt(newItem.quantity, 10)"
                                v-model="newItem.quantity"
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
                        v-for="item in items"
                        :key="item.stableId + '-' + item.stockItemId"
                >
                    <b-td class="col-8 text-center">
                        <span>{{ item.itemName }}</span>
                    </b-td>
                    <b-td class="col-2 text-center">
                        <span
                                v-if="savedItem === undefined || savedItem.stockItemId !== item.stockItemId
                                || savedItem.stableId !== item.stableId"
                        >{{ item.quantity }}</span>
                        <b-form-input
                                v-else
                                size="sm"
                                v-model="item.quantity"
                                type="number"
                                min="0"
                                :state="item.quantity > 0 && item.quantity == parseInt(item.quantity, 10)"
                        ></b-form-input>
                    </b-td>
                    <b-td class="col-2">
                        <font-awesome-icon
                                v-if="savedItem === undefined || savedItem.stockItemId !== item.stockItemId
                                || savedItem.stableId !== item.stableId"
                                class="mx-2"
                                icon="edit"
                                v-on:click="editMode(item.stableId, item.stockItemId)"
                        ></font-awesome-icon>
                        <div v-else>
                            <font-awesome-icon
                                    :class="{ disabled: !updateEnabled}"
                                    class="mx-1"
                                    icon="check"
                                    v-on:click="update(item)"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    class="mx-1"
                                    icon="times"
                                    v-on:click="exitEdit(true)"
                            ></font-awesome-icon>
                            <font-awesome-icon
                                    class="ml-2"
                                    icon="trash-alt"
                                    v-on:click="drop(item)"
                            ></font-awesome-icon>
                        </div>
                    </b-td>
                </b-tr>
            </b-tbody>
        </b-table-simple>
    </b-container>
    <b-container v-else>
        <em class="d-block mx-auto pb-3 text-center">401 Unauthorized - Access can not be granted</em>
    </b-container>
</template>

<script>
    import axios from "axios";

    export default {
        name: "Equipment",
        props: {
            baseUrl: String,
            rights: Object
        },
        mounted: function() {
            this.getAll();
            this.getAllItemTypes();
        },
        data: function () {
            return {
                items: [],
                itemTypes: [],
                newItem: undefined,
                newType: false,
                savedItem: undefined,
                addMode: false
            }
        },
        methods: {
            getAll: function () {
                let that = this;
                axios.get(this.baseUrl + '/stock', { withCredentials: true })
                    .then(function (response) {
                        that.items = response.data;
                    }).catch(error => {
                    that.$emit('error', { title : "Error while retrieving item list", error : error })
                });
            },
            getAllItemTypes: function () {
                let that = this;
                axios.get(this.baseUrl + '/stock/items', { withCredentials: true })
                    .then(function (response) {
                        that.itemTypes = response.data
                    }).catch(error => {
                    that.$emit('error', { title : "Error while retrieving item type list", error : error })
                });
            },
            editMode: function (idStable, idItem) {
                if (this.addMode) {
                    this.unsetAddMode();
                }
                if (this.savedItem !== undefined) {
                    this.exitEdit(true);
                }
                let toSaveItem = this.items.find(
                    item => item.stockItemId === idItem && item.stableId === idStable
                );
                this.savedItem = {
                    stockItemId: toSaveItem.stockItemId,
                    stableId: toSaveItem.stableId,
                    itemName: toSaveItem.itemName,
                    quantity: toSaveItem.quantity
                }
            },
            exitEdit: function (rollback) {
                if (rollback) {
                    let index = this.items.findIndex(
                        item => item.stockItemId === this.savedItem.stockItemId
                            && item.stableId === this.savedItem.stableId
                    );
                    this.items[index] = this.savedItem;
                }
                this.savedItem = undefined;
            },
            update: function (item) {
                if (!this.updateEnabled) {
                    return;
                }
                let that = this;
                let updateData = {
                    quantity : parseInt(item.quantity, 10)
                };
                axios.post(
                    this.baseUrl + '/stock/' + item.stableId + '/' + item.stockItemId,
                    updateData,
                    { withCredentials: true }
                ).then(() => {
                    that.exitEdit(false);
                })
                    .catch(error => {
                        that.$emit('error', { title : "Error while updating stock entry", error : error });
                    })
                ;
            },
            insert: function() {
                if (!this.insertEnabled) {
                    return;
                }
                let that = this;
                let insertData = {
                    StableID: this.newItem.stableId,
                    Quantity: parseInt(this.newItem.quantity, 10),
                    ItemID: parseInt(this.newItem.stockItemId, 10),
                    Item : {
                        itemName: this.newItem.itemName
                    }
                }
                axios.post(this.baseUrl + '/stock/', insertData, { withCredentials: true })
                    .then(() => {
                        that.getAll();
                        that.getAllItemTypes();
                        that.unsetAddMode();
                    })
                    .catch(error => {
                        that.$emit('error', { title : "Error while inserting new item entry", error : error });
                    });
            },
            drop: function (droppedItem) {
                let that = this;
                axios.delete(
                    this.baseUrl + '/stock/' + droppedItem.stableId + '/' + droppedItem.stockItemId,
                    { withCredentials: true }
                ).then(() => {
                    that.exitEdit(false);
                    this.items.splice(
                        this.items.findIndex(
                            item => item.stableId === droppedItem.stableId
                                && item.stockItemId === droppedItem.stockItemId),
                        1)
                }).catch(error => {
                    that.$emit('error', { title : "Error while deleting stock entry", error : error })
                });
            },
            dropType: function (idType) {
                if (this.availableTypes[0] === undefined || idType <= 0 ){
                    return;
                }
                let that = this;
                axios.delete(this.baseUrl + '/stock/items/' + idType, { withCredentials: true })
                    .then(() => {
                        this.itemTypes.splice(this.itemTypes.findIndex(type => type.id === idType), 1)
                        this.newItem.stockItemId = this.availableTypes[0] !== undefined ? this.availableTypes[0].id : 0;
                    }).catch(error => {
                    that.$emit('error', {title: "Error while deleting stock entry", error: error})
                });
            },
            setAddMode: function () {
                if (this.savedItem !== undefined) {
                    this.exitEdit(true);
                }
                this.newItem = {
                    stockItemId: this.availableTypes[0] !== undefined ? this.availableTypes[0].id : 0,
                    stableId: 1,
                    itemName: "",
                    quantity: 1
                }
                this.addMode = true;
            },
            unsetAddMode: function () {
                this.addMode = false;
                this.newType = false;
                this.newItem = undefined;
            },
            setAddNewTypeMode: function () {
                this.newType = true;
                this.newItem.stockItemId = 0;
            },
            unsetAddNewTypeMode: function () {
                this.newType = false;
                this.newItem.itemName = "";
                this.newItem.stockItemId = this.availableTypes[0] !== undefined ? this.availableTypes[0].id : 0;
            }
        },
        computed: {
            insertEnabled: function () {
                // Checking for already existing type in stock
                if (this.newItem.itemName !== undefined
                    && this.items.find(
                        item => item.itemName.toLocaleLowerCase() === this.newItem.itemName.trim().toLowerCase()
                    ) !== undefined) {
                    return false;
                }
                // Checking for already existing type out of stock
                if (this.newItem.itemName !== undefined
                    && this.availableTypes.find(
                        item => item.itemName.toLocaleLowerCase() === this.newItem.itemName.trim().toLowerCase()
                    ) !== undefined) {
                    return false;
                }
                return this.newItem !== undefined && (this.newItem.itemName !== '' || this.newItem.stockItemId !== 0)
                    && this.newItem.quantity >= 0
                    && this.newItem.quantity == parseInt(this.newItem.quantity, 10)
            },
            updateEnabled: function () {
                if (this.savedItem === undefined) {
                    return false;
                }
                let item = this.items.find(
                    item => item.stableId === this.savedItem.stableId
                        && item.stockItemId === this.savedItem.stockItemId
                );
                return item.quantity !== this.savedItem.quantity && item.quantity == parseInt(item.quantity, 10);
            },
            availableTypes: function () {
                let availableTypes = [...this.itemTypes]
                this.items.forEach(item =>
                    availableTypes.splice(
                        availableTypes.findIndex(type => type.id === item.stockItemId)
                        , 1
                    )
                );
                return availableTypes;
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
        background-color: rgba(52, 58, 64, 0.8) !important;
        border-color: #454d55;
        color: rgba(255, 255, 255, 0.8);
    }
</style>