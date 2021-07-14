import { permute, sortBy, reverse, append, map, length, fold, empty, cons, foldBack } from "../../../src/.fable/fable-library.3.1.10/List.js";
import { value } from "../../../src/.fable/fable-library.3.1.10/Option.js";
import { Union } from "../../../src/.fable/fable-library.3.1.10/Types.js";
import { union_type } from "../../../src/.fable/fable-library.3.1.10/Reflection.js";
import { compare, structuralHash, equals, uncurry } from "../../../src/.fable/fable-library.3.1.10/Util.js";
import { List_groupBy } from "../../../src/.fable/fable-library.3.1.10/Seq2.js";

export function filterMap(f, ls) {
    return foldBack((curr, acc) => {
        const matchValue = f(curr);
        if (matchValue == null) {
            return acc;
        }
        else {
            const v = value(matchValue);
            return cons(v, acc);
        }
    }, ls, empty());
}

export class RankStrategy extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["StandardCompetition", "ModifiedCompetition", "DenseRanking"];
    }
}

export function RankStrategy$reflection() {
    return union_type("Tempo.Std.List.RankStrategy", [], RankStrategy, () => [[], [], []]);
}

export function rank(getScore, strategy, ls) {
    const calcRank = (current, groupLength) => {
        switch (strategy.tag) {
            case 1: {
                const assign = (current + groupLength) | 0;
                return [assign, 0];
            }
            case 2: {
                return [current + 1, 0];
            }
            default: {
                return [current + 1, groupLength - 1];
            }
        }
    };
    const ls_2 = fold(uncurry(2, (tupledArg_2) => {
        const currRank = tupledArg_2[0] | 0;
        const ls_1 = tupledArg_2[1];
        return (rest) => {
            const patternInput = calcRank(currRank, length(rest));
            const shift = patternInput[1] | 0;
            const assign_1 = patternInput[0] | 0;
            const accLs = map((v_1) => [assign_1, v_1], rest);
            return [assign_1 + shift, append(ls_1, accLs)];
        };
    }), [0, empty()], map((tupledArg_1) => {
        const v = tupledArg_1[1];
        return v;
    }, reverse(sortBy((tupledArg) => {
        const s = tupledArg[0];
        return s;
    }, List_groupBy(getScore, ls, {
        Equals: (x, y) => equals(x, y),
        GetHashCode: (x) => structuralHash(x),
    }), {
        Compare: (x_1, y_1) => compare(x_1, y_1),
    }))))[1];
    return ls_2;
}

export function moveItem(oldIndex, newIndex, list) {
    if (oldIndex === newIndex) {
        return list;
    }
    else if (oldIndex > newIndex) {
        return permute((index) => ((index === oldIndex) ? newIndex : (((index < newIndex) ? true : (index > oldIndex)) ? index : (index + 1))), list);
    }
    else {
        return permute((index_1) => ((index_1 === oldIndex) ? newIndex : (((index_1 < oldIndex) ? true : (index_1 > newIndex)) ? index_1 : (index_1 - 1))), list);
    }
}

